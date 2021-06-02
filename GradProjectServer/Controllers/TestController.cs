using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using AutoMapper;
using GradProjectServer.Services.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper.Internal;
using GradProjectServer.Common;
using GradProjectServer.Services.Exams.Entities.ExamAttempts;
using GradProjectServer.Services.FilesManagers;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DbManager _dbSeeder;
        private readonly ProgrammingSubQuestionAnswerFileManager _programmingSubQuestionAnswerFileManager;

        public TestController(AppDbContext dbContext, IMapper mapper, DbManager dbSeeder,
            ProgrammingSubQuestionAnswerFileManager programmingSubQuestionAnswerFileManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _dbSeeder = dbSeeder;
            _programmingSubQuestionAnswerFileManager = programmingSubQuestionAnswerFileManager;
        }

        [HttpGet("Seed")]
        public async Task<IActionResult> Seed()
        {
            await _dbSeeder.Seed().ConfigureAwait(false);
            return Ok("Seeded successfully.");
        }

        [HttpGet("RecreateDb")]
        public async Task<IActionResult> RecreateDb()
        {
            await _dbSeeder.RecreateDb().ConfigureAwait(false);
            return Ok("Recreated successfully.");
        }

        [HttpGet("FindExam")]
        public async Task<IActionResult> FindExma([FromQuery] bool? hasMCQ = null, [FromQuery] bool? hasBlank = null,
            [FromQuery] bool? hasProgramming = null)
        {

            var exams = _dbContext.Exams
                .Where(e => e.IsApproved);

            void AddFilter(SubQuestionType type, bool invert = false)
            {
                if (!invert)
                {
                    exams = exams.Where(e =>
                        e.Questions.Any(eq =>
                            eq.ExamSubQuestions.Any(esq =>
                                esq.SubQuestion!.Type == type)));
                }
                else
                {
                    exams = exams.Where(e =>
                        !e.Questions.Any(eq =>
                            eq.ExamSubQuestions.Any(esq =>
                                esq.SubQuestion!.Type == type)));
                }
            }

            if (hasMCQ.HasValue)
            {
                AddFilter(SubQuestionType.MultipleChoice, !hasMCQ.Value);
            }
            if (hasBlank.HasValue)
            {
                AddFilter(SubQuestionType.Blank, !hasBlank.Value);
            }
            if (hasProgramming.HasValue)
            {
                AddFilter(SubQuestionType.Programming, !hasProgramming.Value);
            }

            var exam = await exams.FirstOrDefaultAsync().ConfigureAwait(false);
            if (exam == null)
            {
                return Ok("No exam fulfills your conditions");
            }

            return Ok($"Exam was found, Id: {exam.Id}");
        }

        [LoggedInFilter]
        [HttpGet("AttemptExam")]
        public async Task<IActionResult> AttemptExam([FromQuery] int? examId = null)
        {
            int eId = examId ?? _dbContext.Exams.First(e => e.IsApproved).Id;
            var exam = await _dbContext.Exams
                .Include(e => e.Questions)
                .ThenInclude(e => e.ExamSubQuestions)
                .ThenInclude(e => e.SubQuestion)
                .Include(e => e.Questions)
                .ThenInclude(e => e.Question)
                .ThenInclude(e => e.SubQuestions)
                .FirstOrDefaultAsync(e => e.Id == eId).ConfigureAwait(false);
            if (exam == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Exam not found");
            }

            var user = this.GetUser()!;
            var activeAttempt = await _dbContext.ExamsAttempts.FirstOrDefaultAsync(e => e.OwnerId == user.Id)
                .ConfigureAwait(false);
            if (activeAttempt != null)
            {
                var activeAttemptProgrammingAnswers =
                    _dbContext.ProgrammingSubQuestionAnswers.Where(a => a.AttemptId == activeAttempt.Id);
                foreach (var ans in activeAttemptProgrammingAnswers)
                {
                    _programmingSubQuestionAnswerFileManager.DeleteAnswer(ans);
                }

                _dbContext.ExamsAttempts.Remove(activeAttempt);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }

            ExamAttempt newAttempt = new()
            {
                ExamId = eId,
                OwnerId = user.Id,
                StartTime = DateTimeOffset.Now
            };
            await _dbContext.ExamsAttempts.AddAsync(newAttempt).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            Random rand = new();
            var programmingLanguages = Enum.GetValues<ProgrammingLanguage>();
            await using MemoryStream programmingAnswerStream = new();
            foreach (var eq in exam.Questions.SelectMany(e => e.ExamSubQuestions))
            {
                switch (eq.SubQuestion!.Type)
                {
                    case SubQuestionType.Blank:
                    {
                        await _dbContext.BlankSubQuestionAnswers.AddAsync(new BlankSubQuestionAnswer
                        {
                            Answer = rand.NextText(10),
                            AttemptId = newAttempt.Id,
                            ExamSubQuestionId = eq.Id,
                        });
                        break;
                    }
                    case SubQuestionType.MultipleChoice:
                    {
                        var mcq = await _dbContext.MCQSubQuestions
                            .Include(q => q.Choices)
                            .FirstAsync(e => e.Id == eq.SubQuestion.Id)
                            .ConfigureAwait(false);
                        List<MCQSubQuestionAnswer> selectedChoices = new();
                        var choices = mcq.Choices.ToArray();
                        if (mcq.IsCheckBox)
                        {
                            selectedChoices.Add(new()
                            {
                                ChoiceId = rand.NextElement(choices).Id,
                                AttemptId = newAttempt.Id,
                                ExamSubQuestionId = eq.Id
                            });
                        }
                        else
                        {
                            foreach (var c in choices)
                            {
                                if (rand.NextBool())
                                {
                                    continue;
                                }

                                selectedChoices.Add(new()
                                {
                                    ChoiceId = c.Id,
                                    AttemptId = newAttempt.Id,
                                    ExamSubQuestionId = eq.Id
                                });
                            }
                        }

                        await _dbContext.MCQSubQuestionAnswers.AddRangeAsync(selectedChoices).ConfigureAwait(false);
                        break;
                    }
                    case SubQuestionType.Programming:
                    {
                        bool isZip = rand.NextBool();
                        var answer = (await _dbContext.ProgrammingSubQuestionAnswers.AddAsync(
                                new()
                                {
                                    AttemptId = newAttempt.Id,
                                    FileExtension = isZip ? "zip" : "txt",
                                    ExamSubQuestionId = eq.Id,
                                    ProgrammingLanguage = rand.NextElement(programmingLanguages)
                                })
                            .ConfigureAwait(false)).Entity;
                        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                        programmingAnswerStream.Position = 0;
                        if (isZip)
                        {
                            using var answerArchive =
                                new ZipArchive(programmingAnswerStream, ZipArchiveMode.Create, true);
                            for (int i = 0, e = rand.Next(1, 6); i < e; i++)
                            {
                                var entry = answerArchive.CreateEntry($"e{i}.txt");
                                await using StreamWriter entryWriter = new(entry.Open());
                                await entryWriter.WriteLineAsync($"Hello {answer.Id} answer, file {entry.Name}")
                                    .ConfigureAwait(false);
                                await entryWriter.WriteLineAsync(rand.NextText()).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            await using StreamWriter answerWriter = new(programmingAnswerStream, leaveOpen: true);
                            await answerWriter.WriteLineAsync($"Hello {answer.Id} answer").ConfigureAwait(false);
                            await answerWriter.WriteLineAsync(rand.NextText()).ConfigureAwait(false);
                        }

                        programmingAnswerStream.SetLength(programmingAnswerStream.Position);
                        programmingAnswerStream.Position = 0;
                        await _programmingSubQuestionAnswerFileManager.SaveAnswer(answer, programmingAnswerStream)
                            .ConfigureAwait(false);
                        break;
                    }
                }
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok("Created and filled");
        }
    }
}