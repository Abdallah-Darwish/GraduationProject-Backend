using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GradProjectServer.Common;
using GradProjectServer.DTO;
using GradProjectServer.DTO.ExamAttempts;
using GradProjectServer.DTO.ExamAttempts.SubQuestionGrade;
using GradProjectServer.DTO.ExamSubQuestions;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams;
using GradProjectServer.Services.Exams.Entities.ExamAttempts;
using GradProjectServer.Services.FilesManagers;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Controllers
{
    //todo: delete finished attempts after sometime
    [ApiController]
    [Route("[controller]")]
    public class ExamAttemptController : ControllerBase
    {
        private IQueryable<ExamAttempt> GetPreparedQueryable()
        {
            IQueryable<ExamAttempt> q = _dbContext.ExamsAttempts
                .Include(e => e.Exam)
                .ThenInclude(e => e.Course)
                .Include(e => e.Exam)
                .ThenInclude(e => e.Volunteer)
                .Include(e => e.Owner);

            return q;
        }

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ProgrammingSubQuestionAnswerFileManager _programmingSubQuestionAnswerFileManager;
        private readonly ExamAttemptGrader _examAttemptGrader;

        public ExamAttemptController(AppDbContext dbContext, IMapper mapper,
            ProgrammingSubQuestionAnswerFileManager programmingSubQuestionAnswerFileManager,
            ExamAttemptGrader examAttemptGrader)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _programmingSubQuestionAnswerFileManager = programmingSubQuestionAnswerFileManager;
            _examAttemptGrader = examAttemptGrader;
        }

        /// <summary>
        /// Ids of all exam attempts in data base ordered by exam attempt start time.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        [AdminFilter]
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            return Ok(_dbContext.ExamsAttempts
                .Where(e => (DateTimeOffset.Now - e.StartTime) <= e.Exam.Duration)
                .OrderBy(c => c.StartTime)
                .Skip(info.Offset)
                .Take(info.Count)
                .Select(c => c.Id));
        }

        ///<remarks>
        /// Admin only.
        /// Normal users can use GetActive method.
        /// </remarks>
        /// <param name="examAttemptsIds">Ids of the courses to get.</param>
        /// <response code="404">Ids of the non existing courses.</response>
        [AdminFilter]
        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<ExamAttemptDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<ExamAttemptDto>> Get([FromBody] int[] examAttemptsIds)
        {
            var examAttempts = GetPreparedQueryable();
            var existingExamAttempts = examAttempts
                .Where(e => (DateTimeOffset.Now - e.StartTime) <= e.Exam.Duration)
                .Where(c => examAttemptsIds.Contains(c.Id));
            var nonExistingExamAttempts = examAttemptsIds.Except(existingExamAttempts.Select(c => c.Id)).ToArray();
            if (nonExistingExamAttempts.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following exam attempts don't exist.",
                        Data = new Dictionary<string, object> {["NonExistingExamAttempts"] = nonExistingExamAttempts}
                    });
            }

            return Ok(_mapper.ProjectTo<IEnumerable<ExamAttemptDto>>(existingExamAttempts));
        }

        ///<remarks>
        /// Returns the user currently active exam attempt.
        /// It might be over but still saved for some time for the user to grade it.
        /// </remarks>
        /// <response code="200">The user has an active attempt.</response>
        /// <response code="204">The user has NO active attempts.</response>
        [LoggedInFilter]
        [HttpGet("GetActive")]
        [ProducesResponseType(typeof(ExamAttemptDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExamAttemptDto), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetActive()
        {
            var user = this.GetUser()!;
            var examAttempts = GetPreparedQueryable();
            var attempt = await examAttempts.FirstOrDefaultAsync(e => e.OwnerId == user.Id).ConfigureAwait(false);
            if (attempt == null || attempt.IsOver)
            {
                return NoContent();
            }

            return Ok(_mapper.Map<ExamAttemptDto>(attempt));
        }

        /// <ramarks>
        /// Creates a new exam attempt for user but he can't have any active attempts.
        /// </ramarks>
        /// <param name="examId"></param>
        /// <returns></returns>
        [LoggedInFilter]
        [HttpGet("Create")]
        [ProducesResponseType(typeof(ExamAttemptDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromQuery] int examId)
        {
            var user = this.GetUser()!;
           
            var exam = await _dbContext.Exams.FindAsync(examId).ConfigureAwait(false);
            if (exam == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "There is no exam with the specified id.",
                        Data = new() {["ExamId"] = examId}
                    });
            }
            
            var examsAttempts = GetPreparedQueryable();
            
            var activeAttempt = await examsAttempts
                .FirstOrDefaultAsync(e => e.OwnerId == user.Id)
                .ConfigureAwait(false);
            if (activeAttempt != null)
            {
                if (activeAttempt.IsOver)
                {
                    var activeAttemptProgrammingAnswers =
                        _dbContext.ProgrammingSubQuestionAnswers.Where(a => a.AttemptId == activeAttempt.Id);
                    foreach (var ans in activeAttemptProgrammingAnswers)
                    {
                        _programmingSubQuestionAnswerFileManager.DeleteAnswer(ans);
                    }

                    _dbContext.ExamsAttempts.Remove(activeAttempt);
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "Please finish your currently active attempt first.",
                            Data = new() {["ActiveAttemptId"] = activeAttempt.Id}
                        });
                }
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            ExamAttempt newAttempt = new()
            {
                ExamId = examId,
                OwnerId = user.Id,
                StartTime = DateTimeOffset.Now
            };
            await _dbContext.ExamsAttempts.AddAsync(newAttempt).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            var newAttemptEntity =
                await GetPreparedQueryable().FirstAsync(e => e.OwnerId == user.Id).ConfigureAwait(false);
            return StatusCode(StatusCodes.Status201Created, _mapper.Map<ExamAttemptDto>(newAttemptEntity));
        }

        /// <summary>
        /// Finishes the user current(not necessarily active) attempt. 
        /// </summary>
        /// <response code="403">User has no exam attempts to finish.</response>
        [LoggedInFilter]
        [HttpPost("FinishCurrent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> FinishCurrent()
        {
            var user = this.GetUser()!;
            var examAttempts = GetPreparedQueryable();
            var attempt = await examAttempts.FirstOrDefaultAsync(e => e.OwnerId == user.Id).ConfigureAwait(false);
            if (attempt == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorDTO
                    {
                        Description = "User has no exam attempts to finish.",
                    });
            }

            attempt.IsFinished = true;
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        [LoggedInFilter]
        [HttpGet("GradeCurrent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO),StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GradeCurrent()
        {
            var user = this.GetUser()!;
            var examAttempts = GetPreparedQueryable()
                .Include(a => a.Answers)
                .ThenInclude(a => a.ExamSubQuestion)
                .ThenInclude(e => e.SubQuestion);
            var attempt = await examAttempts.FirstOrDefaultAsync(e => e.OwnerId == user.Id).ConfigureAwait(false);
            if (attempt == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorDTO
                    {
                        Description = "User has no exam attempts to grade.",
                    });
            }

            ExamAttemptGradeDto result = new();
            List<AnswerGradeDto> answersGrades = new();
            HashSet<int> seenMCQQuestions = new();
            foreach (var answer in attempt.Answers)
            {
                switch (answer.ExamSubQuestion!.SubQuestion!.Type)
                {
                    case SubQuestionType.Blank:
                        var blank = await _dbContext.BlankSubQuestions.FindAsync(answer.ExamSubQuestion.SubQuestionId)
                            .ConfigureAwait(false);
                        var blankAnswer = await _dbContext.BlankSubQuestionAnswers.FindAsync(answer.Id)
                            .ConfigureAwait(false);
                        var blankGrade = await _examAttemptGrader.Grade(blankAnswer).ConfigureAwait(false);
                        answersGrades.Add(new BlankAnswerGradeDto()
                        {
                            Grade = blankGrade.Grade,
                            Comment = blankGrade.Comment,
                            Weight = answer.ExamSubQuestion.Weight,
                            KeyAnswer = blank.Answer,
                            UserAnswer = blankAnswer.Answer,
                            SubQuestion = _mapper.Map<ExamSubQuestionDto>(answer.ExamSubQuestion)
                        });
                        break;
                    case SubQuestionType.Programming:
                        var pro = await _dbContext.ProgrammingSubQuestionAnswers.FindAsync(answer.Id)
                            .ConfigureAwait(false);
                        var proGrade = await _examAttemptGrader.Grade(pro).ConfigureAwait(false);
                        answersGrades.Add(new ProgrammingAnswerGradeDto()
                        {
                            Grade = proGrade.Grade,
                            Comment = proGrade.Comment,
                            Weight = answer.ExamSubQuestion.Weight,
                            SubQuestion = _mapper.Map<ExamSubQuestionDto>(answer.ExamSubQuestion),
                            Verdict = proGrade.Verdict
                        });
                        break;
                    case SubQuestionType.MultipleChoice:
                        if (seenMCQQuestions.Contains(answer.ExamSubQuestionId))
                        {
                            continue;
                        }

                        var userChoices = _dbContext.MCQSubQuestionAnswers.Include(c => c.Choice)
                            .Where(c =>
                                c.AttemptId == attempt.Id && c.ExamSubQuestionId == answer.ExamSubQuestionId).ToArray();
                        var mcq = await _dbContext.MCQSubQuestions.Include(q => q.Choices)
                            .FirstAsync(q => q.Id == answer.ExamSubQuestion.SubQuestionId).ConfigureAwait(false);
                        var grade = await _examAttemptGrader.Grade(userChoices).ConfigureAwait(false);
                        answersGrades.Add(new MCQAnswerGradeDto()
                        {
                            Grade = grade,
                            Weight = answer.ExamSubQuestion.Weight,
                            SubQuestion = _mapper.Map<ExamSubQuestionDto>(answer.ExamSubQuestion),
                            UserChoices =
                                _mapper.Map<OwnedMCQSubQuestionChoiceDto[]>(userChoices.Select(c => c.Choice)
                                    .ToArray()),
                            CorrectChoices =
                                _mapper.Map<OwnedMCQSubQuestionChoiceDto[]>(mcq.Choices.Where(c => c.Weight > 0.0f)
                                    .ToArray())
                        });
                        break;
                }
            }
            foreach (var grade in answersGrades)
            {
                grade.Grade *= grade.Weight;
                result.Grade += grade.Grade;
                result.Weight += grade.Weight;
            }

            result.AnswersGrades = answersGrades.ToArray();
            return Ok(result);
        }
    }
}