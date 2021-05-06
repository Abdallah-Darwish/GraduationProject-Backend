using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GradProjectServer.Common;
using GradProjectServer.DTO;
using GradProjectServer.DTO.ExamAttempts.SubQuestionGrade;
using GradProjectServer.DTO.ExamSubQuestions;
using GradProjectServer.DTO.SubQuestionAnswers;
using GradProjectServer.DTO.SubQuestionAnswers.Blank;
using GradProjectServer.DTO.SubQuestionAnswers.MCQ;
using GradProjectServer.DTO.SubQuestionAnswers.Programming;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities.ExamAttempts;
using GradProjectServer.Services.FilesManagers;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubQuestionAnswerController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ProgrammingSubQuestionAnswerFileManager _programmingSubQuestionAnswerFileManager;

        public SubQuestionAnswerController(AppDbContext dbContext, IMapper mapper,
            ProgrammingSubQuestionAnswerFileManager programmingSubQuestionAnswerFileManager)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _programmingSubQuestionAnswerFileManager = programmingSubQuestionAnswerFileManager;
        }

        /// <param name="examSubQuestionId">Id of the sub question IN EXAM that you want to get your recorded answer to it.</param>
        ///<response code="200">
        /// The recorded answer for this question, and its type is:
        /// 1- BlankSubQuestionAnswerDto if its complete the blank question.
        /// 2- MCQSubQuestionAnswerDto if its an MCQ.
        /// 3- A stream of bytes that contains the answer file if its a programming question, with header Content-Type: application/octet-stream.
        ///</response>
        /// <response code="204">If the user haven't answered this question yet.</response>
        /// <response code="403">If the user doesn't have any active exam attempt.</response>
        /// <response code="404">If the active exam attempt for this user doesn't have a sub question with the specified id.</response>
        [LoggedInFilter]
        [HttpGet("Get")]
        [ProducesResponseType(typeof(BlankSubQuestionAnswerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MCQSubQuestionAnswerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromQuery] int examSubQuestionId)
        {
            var user = this.GetUser()!;
            var attempt = await _dbContext.ExamsAttempts.FirstAsync(e => e.OwnerId == user.Id).ConfigureAwait(false);
            if (attempt == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorDTO
                    {
                        Description = "User doesn't have any active exam attempts."
                    });
            }

            var examId = attempt.ExamId;
            var subQuestion = await _dbContext.ExamSubQuestions
                .Include(e => e.SubQuestion)
                .FirstAsync(q => q.ExamQuestion.ExamId == examId && q.SubQuestionId == examSubQuestionId)
                .ConfigureAwait(false);
            if (subQuestion == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The exam doesn't have any sub question with the following id.",
                        Data = new()
                        {
                            ["ExamId"] = examId,
                            ["ExamSubQuestionId"] = examSubQuestionId
                        }
                    });
            }

            var attemptId = attempt.Id;

            if (subQuestion.SubQuestion.Type == SubQuestionType.Blank)
            {
                var blanksAnswers = _dbContext.BlankSubQuestionAnswers
                    .Include(e => e.SubQuestion)
                    .ThenInclude(e => e.SubQuestion);
                var answer = await blanksAnswers
                    .FirstAsync(e => e.AttemptId == attemptId && e.ExamSubQuestionId == examSubQuestionId)
                    .ConfigureAwait(false);
                if (answer == null)
                {
                    return NoContent();
                }

                BlankSubQuestionAnswerDto result = new()
                {
                    SubQuestion = _mapper.Map<ExamSubQuestionDto>(answer.SubQuestion),
                    Answer = answer.Answer
                };
                return Ok(result);
            }

            if (subQuestion.SubQuestion.Type == SubQuestionType.MultipleChoice)
            {
                var mcqAnswers = _dbContext.MCQSubQuestionAnswers
                    .Include(e => e.SubQuestion)
                    .ThenInclude(e => e.SubQuestion)
                    .Include(e => e.Choice);
                var answers = await mcqAnswers
                    .Where(e => e.AttemptId == attemptId && e.ExamSubQuestionId == examSubQuestionId)
                    .ToArrayAsync()
                    .ConfigureAwait(false);
                if (answers.Length == 0)
                {
                    return NoContent();
                }

                MCQSubQuestionAnswerDto result = new()
                {
                    SubQuestion = _mapper.Map<ExamSubQuestionDto>(answers[0].SubQuestion),
                    SelectedChoices = _mapper.Map<MCQSubQuestionChoiceDto[]>(answers.Select(a => a.Choice).ToArray()),
                };
                return Ok(result);
            }

            {
                var programmingAnswers = _dbContext.ProgrammingSubQuestionAnswers
                    .Include(e => e.SubQuestion)
                    .ThenInclude(e => e.SubQuestion);
                var answer = await programmingAnswers
                    .FirstAsync(e => e.AttemptId == attemptId && e.ExamSubQuestionId == examSubQuestionId)
                    .ConfigureAwait(false);
                if (answer == null)
                {
                    return NoContent();
                }

                ProgrammingSubQuestionAnswerDto result = new()
                {
                    SubQuestion = _mapper.Map<ExamSubQuestionDto>(answer.SubQuestion),
                    ProgrammingLanguage = answer.ProgrammingLanguage
                };
                return Ok(result);
            }
        }

        [NonAction]
        public async Task<IActionResult> GetProgrammingSubQuestionAnswerFile([FromQuery] int examSubQuestionId)
        {
            var user = this.GetUser()!;
            var attempt = await _dbContext.ExamsAttempts
                .FirstOrDefaultAsync(e => e.OwnerId == user.Id)
                .ConfigureAwait(false);
            if (attempt == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorDTO
                    {
                        Description = "The user doesn't have any exam attempts.",
                    });
            }

            var examSubQuestion =
                await _dbContext.ExamSubQuestions
                    .Include(e => e.SubQuestion)
                    .Include(e => e.ExamQuestion)
                    .FirstOrDefaultAsync(e =>
                        e.Id == examSubQuestionId && e.SubQuestion.Type == SubQuestionType.Programming)
                    .ConfigureAwait(false);
            if (examSubQuestion == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description =
                            "The current user attempt doesn't have any programming sub question with the following id.",
                        Data = new()
                        {
                            ["ExamSubQuestionId"] = examSubQuestionId
                        }
                    });
            }

            if (examSubQuestion.ExamQuestion.ExamId != attempt.ExamId)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorDTO
                    {
                        Description =
                            "The requested programming exam sub question doesn't belong to the user active attempt.",
                        Data = new()
                        {
                            ["ProgrammingExamSubQuestionId"] = examSubQuestionId
                        }
                    });
            }

            var answer = await _dbContext.ProgrammingSubQuestionAnswers
                .FirstOrDefaultAsync(e =>
                    e.Attempt.OwnerId == user.Id && e.SubQuestion.SubQuestion.Type == SubQuestionType.Programming &&
                    e.ExamSubQuestionId == examSubQuestionId)
                .ConfigureAwait(false);
            if (answer == null)
            {
                return NoContent();
            }

            return File(_programmingSubQuestionAnswerFileManager.GetAnswer(answer), "application/octet-stream",
                $"{examSubQuestion}Answer.{answer.FileExtension}");
        }

        /// <remarks>
        /// Records student answer for a sub question in the active exam attempt.
        /// After calling this method any previous answers will be deleted.
        /// </remarks>
        /// <param name="answer">
        /// The answer you want to record, it can be of type:
        /// 1- CreateBlankSubQuestionAnswerDto.
        /// 2- CreateMCQSubQuestionAnswerDto.
        /// 3- CreateProgrammingSubQuestionAnswerDto
        /// </param>
        /// <response code="200">If every thing works fine and the new answer is recorded.</response>
        [LoggedInFilter]
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateSubQuestionAnswerDto answer)
        {
            var user = this.GetUser()!;
            var attempt = await _dbContext.ExamsAttempts.FirstAsync(e => e.OwnerId == user.Id).ConfigureAwait(false);
            if (attempt == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorDTO
                    {
                        Description = "User doesn't have any active exam attempts."
                    });
            }

            var examId = attempt.ExamId;
            var examSubQuestionId = answer.ExamSubQuestionId;
            var subQuestion = await _dbContext.ExamSubQuestions
                .Include(e => e.SubQuestion)
                .FirstAsync(q => q.ExamQuestion.ExamId == examId && q.SubQuestionId == examSubQuestionId)
                .ConfigureAwait(false);
            if (subQuestion == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The exam doesn't have any sub question with the following id.",
                        Data = new()
                        {
                            ["ExamId"] = examId,
                            ["ExamSubQuestionId"] = examSubQuestionId
                        }
                    });
            }

//todo: all of the validation here should be inside fluent validation
            ObjectResult CreateUnexpectedAnswerResult()
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity,
                    new ErrorDTO
                    {
                        Description = "Unexpected answer type.",
                        Data = new()
                        {
                            ["ExpectedType"] = subQuestion.SubQuestion.Type switch
                            {
                                SubQuestionType.Blank => nameof(CreateBlankSubQuestionAnswerDto),
                                SubQuestionType.Programming => nameof(CreateProgrammingSubQuestionDto),
                                SubQuestionType.MultipleChoice => nameof(CreateMCQSubQuestionAnswerDto),
                            },
                            ["ActualAnswerType"] = answer.GetType().Name
                        }
                    });
            }

            var attemptId = attempt.Id;

            if (subQuestion.SubQuestion.Type == SubQuestionType.Blank)
            {
                if (!(answer is CreateBlankSubQuestionAnswerDto blankAnswer))
                {
                    return CreateUnexpectedAnswerResult();
                }

                var previousAnswer = await _dbContext.SubQuestionAnswers
                    .FirstAsync(e => e.AttemptId == attemptId && e.ExamSubQuestionId == examSubQuestionId)
                    .ConfigureAwait(false);
                _dbContext.SubQuestionAnswers.Remove(previousAnswer);
                BlankSubQuestionAnswer entity = new()
                {
                    Answer = blankAnswer.Answer,
                    AttemptId = attemptId,
                    ExamSubQuestionId = subQuestion.Id
                };
                await _dbContext.BlankSubQuestionAnswers.AddAsync(entity).ConfigureAwait(false);
            }
            else if (subQuestion.SubQuestion.Type == SubQuestionType.MultipleChoice)
            {
                if (!(answer is CreateMCQSubQuestionAnswerDto mcqAnswer))
                {
                    return CreateUnexpectedAnswerResult();
                }

                var existingChoices = _dbContext.MCQSubQuestionsChoices
                    .Include(c => c.SubQuestion)
                    .Where(c => mcqAnswer.SelectedChoices.Contains(c.Id));
                var nonExistingChoices = mcqAnswer.SelectedChoices.Except(existingChoices.Select(c => c.Id)).ToArray();
                if (nonExistingChoices.Length > 0)
                {
                    return StatusCode(StatusCodes.Status422UnprocessableEntity,
                        new ErrorDTO
                        {
                            Description = "The following choices don't exist.",
                            Data = new()
                            {
                                ["NonExistingChoices"] = nonExistingChoices,
                            }
                        });
                }

                var subQuestionId = subQuestion.SubQuestionId;
                var notBelongingChoices = existingChoices.Where(c => c.SubQuestion.Id != subQuestionId).ToArray();
                if (notBelongingChoices.Length > 0)
                {
                    return StatusCode(StatusCodes.Status422UnprocessableEntity,
                        new ErrorDTO
                        {
                            Description = "The following choices don't belong to the question you want to answer.",
                            Data = new()
                            {
                                ["NotBelongingChoices"] = notBelongingChoices,
                            }
                        });
                }

                var previousAnswers = _dbContext.SubQuestionAnswers
                    .Where(e => e.AttemptId == attemptId && e.ExamSubQuestionId == examSubQuestionId);
                _dbContext.SubQuestionAnswers.RemoveRange(previousAnswers);
                var entities = mcqAnswer.SelectedChoices
                    .Select(id => new MCQSubQuestionAnswer()
                    {
                        AttemptId = attemptId,
                        ExamSubQuestionId = examSubQuestionId,
                        ChoiceId = id
                    });
                await _dbContext.MCQSubQuestionAnswers.AddRangeAsync(entities).ConfigureAwait(false);
            }
            else
            {
                if (!(answer is CreateProgrammingSubQuestionAnswerDto programmingAnswer))
                {
                    return CreateUnexpectedAnswerResult();
                }

                var previousAnswer = await _dbContext.ProgrammingSubQuestionAnswers
                    .FirstAsync(e => e.AttemptId == attemptId && e.ExamSubQuestionId == examSubQuestionId)
                    .ConfigureAwait(false);
                _programmingSubQuestionAnswerFileManager.DeleteAnswer(previousAnswer);
                _dbContext.SubQuestionAnswers.Remove(previousAnswer);
                ProgrammingSubQuestionAnswer entity = new()
                {
                    FileExtension = programmingAnswer.Answer.FileExtension,
                    ProgrammingLanguage = programmingAnswer.ProgrammingLanguage,
                    AttemptId = attemptId,
                    ExamSubQuestionId = subQuestion.Id
                };
                await _dbContext.ProgrammingSubQuestionAnswers.AddAsync(entity).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                await using var answerStream = await Utility.DecodeBase64Async(programmingAnswer.Answer.ContentBase64)
                    .ConfigureAwait(false);
                await _programmingSubQuestionAnswerFileManager.SaveAnswer(entity, answerStream);
            }

            return Ok();
        }
    }
}