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

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExamSubQuestionController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public ExamSubQuestionController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Get([FromBody] int[] examSubQuestionsIds)
        {
            var existingExamSubQuestions = _dbContext.ExamsSubQuestions.Where(c => examSubQuestionsIds.Contains(c.Id));
            var nonExistingExamSubQuestions = examSubQuestionsIds.Except(existingExamSubQuestions.Select(c => c.Id)).ToArray();
            if (nonExistingExamSubQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following exam sub questions don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingExamSubQuestions"] = nonExistingExamSubQuestions }
                        });
            }
            var user = this.GetUser();
            if (!(user?.IsAdmin ?? false))
            {
                int userId = user?.Id ?? -1;
                var notOwnedExamSubQuestions = existingExamSubQuestions
                    .Where(e => e.ExamQuestion.Exam.VolunteerId != userId && !e.ExamQuestion.Exam.IsApproved)
                    .Select(q => q.Id)
                    .ToArray();
                if (notOwnedExamSubQuestions.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following not approved exam sub questions.",
                            Data = new Dictionary<string, object> { ["NotOwnedNotApprovedExamQuestions"] = notOwnedExamSubQuestions }
                        });
                }
            }
            return Ok(_mapper.ProjectTo<ExamSubQuestionDto>(existingExamSubQuestions));
        }
        [HttpDelete]
        [LoggedInFilter]
        public async Task<IActionResult> Delete([FromBody] int[] examSubQuestionsIds)
        {
            var existingExamSubQuestions = _dbContext.ExamsSubQuestions.Where(c => examSubQuestionsIds.Contains(c.Id));
            var nonExistingExamSubQuestions = examSubQuestionsIds.Except(existingExamSubQuestions.Select(c => c.Id)).ToArray();
            if (nonExistingExamSubQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following exam sub questions don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingExamSubQuestions"] = nonExistingExamSubQuestions }
                        });
            }
            var user = this.GetUser()!;
            if (!user.IsAdmin)
            {
                var approvedExamQuestionsIds = existingExamSubQuestions.Where(sq => sq.ExamQuestion.Exam.IsApproved).Select(sq => sq.Id).ToArray();
                return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "The following exam sub questions are already approved so they can't be updated.",
                            Data = new Dictionary<string, object> { ["ApprovedExamSubQuestions"] = approvedExamQuestionsIds }
                        });
            }
            _dbContext.ExamsSubQuestions.RemoveRange(existingExamSubQuestions);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [HttpPost]
        [LoggedInFilter]
        public async Task<IActionResult> Create([FromBody] CreateExamSubQuestionDto dto)
        {
            var examSubQuestion = new ExamSubQuestion
            {
                ExamQuestionId = dto.ExamQuestionId,
                SubQuestionId = dto.SubQuestionId,
                Weight = dto.Weight
            };
            await _dbContext.ExamsSubQuestions.AddAsync(examSubQuestion).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new { examSubQuestionsIds = new int[] { examSubQuestion.Id } }, _mapper.Map<ExamSubQuestionDto>(examSubQuestion));
        }
        [HttpPatch]
        [LoggedInFilter]
        public async Task<IActionResult> Update([FromBody] UpdateExamSubQuestionDto update)
        {
            var examSubQuestion = await _dbContext.ExamsSubQuestions.FindAsync(update.Id).ConfigureAwait(false);
            if (update.Weight.HasValue)
            {
                examSubQuestion.Weight = update.Weight.Value;
            }
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}
