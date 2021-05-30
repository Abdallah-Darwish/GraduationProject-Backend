using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.ExamQuestions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExamQuestionController : ControllerBase
    {
        private IQueryable<ExamQuestion> GetPreparedQueryable()
        {
            var q = _dbContext.ExamsQuestions.Include(e => e.Question)
                .Include(e => e.ExamSubQuestions)
                .ThenInclude(e => e.SubQuestion);
            return q;
        }

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ExamQuestionController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Result is ordered by Id.</summary>
        /// <remarks>
        /// A user has access to:
        ///     1- All approved exam questions.
        ///     2- HIS not exam questions.
        /// An admin has access to:
        ///     All exam questions.
        /// </remarks>
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            var user = this.GetUser();
            var examQuestions = _dbContext.ExamsQuestions.AsQueryable();
            if (!(user?.IsAdmin ?? false))
            {
                var userId = user?.Id ?? -1;
                examQuestions = examQuestions.Where(e => e.Exam.VolunteerId == userId || e.Exam.IsApproved);
            }

            return Ok(examQuestions.Skip(info.Offset).Take(info.Count).Select(e => e.Id));
        }

        /// <param name="examQuestionsIds">Ids of the exam questions to get.</param>
        /// <remarks>
        /// A user has access to:
        ///     1- All approved exam questions.
        ///     2- HIS not approved exam questions.
        /// An admin has access to:
        ///     All exam questions.
        /// </remarks>
        /// <response code="404">Ids of the non existing exam questions.</response>
        /// <response code="403">Ids of exam questions the user has no access rights to.</response>
        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<ExamQuestion>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public ActionResult<IEnumerable<ExamQuestion>> Get([FromBody] int[] examQuestionsIds)
        {
            var examQuestions = GetPreparedQueryable();
            var existingExamQuestions = examQuestions.Where(c => examQuestionsIds.Contains(c.Id));
            var nonExistingExamQuestions = examQuestionsIds.Except(existingExamQuestions.Select(c => c.Id)).ToArray();
            if (nonExistingExamQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following exam questions don't exist.",
                        Data = new Dictionary<string, object> {["NonExistingExamQuestions"] = nonExistingExamQuestions}
                    });
            }

            var user = this.GetUser();
            if (!(user?.IsAdmin ?? false))
            {
                int userId = user?.Id ?? -1;
                var notOwnedExamQuestions = existingExamQuestions
                    .Where(e => e.Exam.VolunteerId != userId && !e.Exam.IsApproved)
                    .Select(q => q.Id)
                    .ToArray();
                if (notOwnedExamQuestions.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following not approved exam questions.",
                            Data = new Dictionary<string, object>
                                {["NotOwnedNotApprovedExamQuestions"] = notOwnedExamQuestions}
                        });
                }
            }

            return Ok(_mapper.ProjectTo<ExamQuestionDto>(existingExamQuestions));
        }

        /// <summary>Deletes the specified exam questions.</summary>
        /// <param name="examQuestionsIds">Ids of exam question to delete.</param>
        /// <remarks>
        /// A user can delete:
        ///     HIS NOT approved exam questions.
        /// An admin can delete:
        ///     All exam questions.
        /// </remarks>
        /// <response code="404">Ids of the non existing exam questions.</response>
        /// <response code="403">Ids of the exam questions user can't modify.</response>
        [LoggedInFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete([FromBody] int[] examQuestionsIds)
        {
            var existingExamQuestions = _dbContext.ExamsQuestions.Where(c => examQuestionsIds.Contains(c.Id));
            var nonExistingExamQuestions = examQuestionsIds.Except(existingExamQuestions.Select(c => c.Id)).ToArray();
            if (nonExistingExamQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following exam questions don't exist.",
                        Data = new Dictionary<string, object>
                        {
                            ["NonExistingExamQuestions"] = nonExistingExamQuestions
                        }
                    });
            }

            var user = this.GetUser()!;
            if (!user.IsAdmin)
            {
                var approvedExamQuestionsIds =
                    existingExamQuestions.Where(q => q.Exam.IsApproved).Select(q => q.Id).ToArray();
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorDTO
                    {
                        Description = "The following exam questions are already approved so they can't be updated.",
                        Data = new Dictionary<string, object> {["ApprovedExamQuestions"] = approvedExamQuestionsIds}
                    });
            }

            _dbContext.ExamsQuestions.RemoveRange(existingExamQuestions);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>Creates a new exam question.</summary>
        /// <response code="201">The newly created exam question.</response>
        [LoggedInFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ExamSubQuestion), StatusCodes.Status201Created)]
        public async Task<ActionResult<ExamQuestion>> Create([FromBody] CreateExamQuestionDto dto)
        {
            var examQuestion = new ExamQuestion
            {
                ExamId = dto.ExamId,
                Order = dto.Order,
                QuestionId = dto.QuestionId
            };
            await _dbContext.ExamsQuestions.AddAsync(examQuestion).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            examQuestion =
                await GetPreparedQueryable().FirstAsync(q => q.Id == examQuestion.Id).ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new {examQuestionsIds = new int[] {examQuestion.Id}},
                _mapper.Map<ExamQuestionDto>(examQuestion));
        }

       //No update because also you need to update sub question so just recreate.
    }
}