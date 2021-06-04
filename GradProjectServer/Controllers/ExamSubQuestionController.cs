using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.ExamSubQuestions;
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
    public class ExamSubQuestionController : ControllerBase
    {
        private IQueryable<ExamSubQuestion> GetPreparedQueryable()
        {
            var q = _dbContext.ExamSubQuestions
                .Include(e => e.SubQuestion);
            return q;
        }

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ExamSubQuestionController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Result is ordered by Id.</summary>
        /// <remarks>
        /// A user has access to:
        ///     1- All approved exam sub questions.
        ///     2- HIS not exam sub questions.
        /// An admin has access to:
        ///     All exam sub questions.
        /// </remarks>
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            var user = this.GetUser();
            var examSubQuestions = _dbContext.ExamSubQuestions.AsQueryable();
            if (!(user?.IsAdmin ?? false))
            {
                var userId = user?.Id ?? -1;
                examSubQuestions = examSubQuestions.Where(e =>
                    e.ExamQuestion.Exam.VolunteerId == userId || e.ExamQuestion.Exam.IsApproved);
            }

            return Ok(examSubQuestions.Skip(info.Offset).Take(info.Count).Select(e => e.Id));
        }

        /// <param name="examSubQuestionsIds">Ids of the exam sub questions to get.</param>
        /// <remarks>
        /// A user has access to:
        ///     1- All approved exam sub questions.
        ///     2- HIS not exam sub questions.
        /// An admin has access to:
        ///     All exam sub questions.
        /// </remarks>
        /// <response code="404">Ids of the non existing exam questions.</response>
        /// <response code="403">Ids of exam questions the user has no access rights to.</response>
        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<ExamSubQuestionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ExamSubQuestionDto>>> Get([FromBody] int[] examSubQuestionsIds)
        {
            var existingExamSubQuestions = GetPreparedQueryable().Where(c => examSubQuestionsIds.Contains(c.Id));
            var nonExistingExamSubQuestions =
                examSubQuestionsIds.Except(existingExamSubQuestions.Select(c => c.Id)).ToArray();
            if (nonExistingExamSubQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following exam sub questions don't exist.",
                        Data = new Dictionary<string, object>
                            {["NonExistingExamSubQuestions"] = nonExistingExamSubQuestions}
                    });
            }

            var user = this.GetUser();
            if (!(user?.IsAdmin ?? false))
            {
                int userId = user?.Id ?? -1;
                var notOwnedExamSubQuestions = await existingExamSubQuestions
                    .Where(e => e.ExamQuestion.Exam.VolunteerId != userId && !e.ExamQuestion.Exam.IsApproved)
                    .Select(q => q.Id)
                    .ToArrayAsync()
                    .ConfigureAwait(false);
                if (notOwnedExamSubQuestions.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following not approved exam sub questions.",
                            Data = new Dictionary<string, object>
                                {["NotOwnedNotApprovedExamQuestions"] = notOwnedExamSubQuestions}
                        });
                }
            }

            return Ok(_mapper.ProjectTo<ExamSubQuestionDto>(existingExamSubQuestions));
        }

        /// <summary>Deletes the specified exam sub questions.</summary>
        /// <param name="examSubQuestionsIds">Ids of exam sub question to delete.</param>
        /// <remarks>
        /// A user can delete:
        ///     HIS NOT approved exam sub questions.
        /// An admin can delete:
        ///     All exam sub questions.
        /// </remarks>
        /// <response code="404">Ids of the non existing exam sub questions.</response>
        /// <response code="403">Ids of the exam sub questions user can't modify.</response>
        [LoggedInFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete([FromBody] int[] examSubQuestionsIds)
        {
            var existingExamSubQuestions = _dbContext.ExamSubQuestions.Where(c => examSubQuestionsIds.Contains(c.Id));
            var nonExistingExamSubQuestions =
                examSubQuestionsIds.Except(existingExamSubQuestions.Select(c => c.Id)).ToArray();
            if (nonExistingExamSubQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following exam sub questions don't exist.",
                        Data = new Dictionary<string, object>
                        {
                            ["NonExistingExamSubQuestions"] = nonExistingExamSubQuestions
                        }
                    });
            }

            var user = this.GetUser()!;
            if (!user.IsAdmin)
            {
                var approvedExamQuestionsIds = existingExamSubQuestions.Where(sq => sq.ExamQuestion.Exam.IsApproved)
                    .Select(sq => sq.Id).ToArray();
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorDTO
                    {
                        Description = "The following exam sub questions are already approved so they can't be updated.",
                        Data = new Dictionary<string, object>
                        {
                            ["ApprovedExamSubQuestions"] = approvedExamQuestionsIds
                        }
                    });
            }

            _dbContext.ExamSubQuestions.RemoveRange(existingExamSubQuestions);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>Creates a new exam sub question.</summary>
        /// <response code="201">The newly created exam sub question.</response>
        [LoggedInFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ExamSubQuestionDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateExamSubQuestionDto dto)
        {
            var examSubQuestion = new ExamSubQuestion
            {
                ExamQuestionId = dto.ExamQuestionId,
                SubQuestionId = dto.SubQuestionId,
                Weight = dto.Weight,
                Order = dto.Order
            };
            await _dbContext.ExamSubQuestions.AddAsync(examSubQuestion).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            examSubQuestion = await GetPreparedQueryable().FirstAsync(sq => sq.Id == examSubQuestion.Id)
                .ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new {examSubQuestionsIds = new int[] {examSubQuestion.Id}},
                _mapper.Map<ExamSubQuestionDto>(examSubQuestion));
        }

        /// <summary>Updates an exam sub question.</summary>
        [LoggedInFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateExamSubQuestionDto update)
        {
            var examSubQuestion = await _dbContext.ExamSubQuestions.FindAsync(update.Id).ConfigureAwait(false);
            if (update.Weight.HasValue)
            {
                examSubQuestion.Weight = update.Weight.Value;
            }

            if (update.Order.HasValue)
            {
                examSubQuestion.Order = update.Order.Value;
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}