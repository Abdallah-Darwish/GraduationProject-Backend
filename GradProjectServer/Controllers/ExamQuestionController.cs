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

namespace GradProjectServer.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ExamQuestionController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public ExamQuestionController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Get([FromBody] int[] examQuestionsIds)
        {
            var existingExamQuestions = _dbContext.ExamsQuestions.Where(c => examQuestionsIds.Contains(c.Id));
            var nonExistingExamQuestions = examQuestionsIds.Except(existingExamQuestions.Select(c => c.Id)).ToArray();
            if (nonExistingExamQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following exam questions don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingExamQuestions"] = nonExistingExamQuestions }
                        });
            }
            var user = this.GetUser();
            if(!(user?.IsAdmin ?? false))
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
                            Data = new Dictionary<string, object> { ["NotOwnedNotApprovedExamQuestions"] = notOwnedExamQuestions }
                        });
                }
            }
            return Ok(_mapper.ProjectTo<ExamQuestionDto>(existingExamQuestions));
        }
        [HttpDelete]
        [LoggedInFilter]
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
                            Data = new Dictionary<string, object> { ["NonExistingExamQuestions"] = nonExistingExamQuestions }
                        });
            }
            var user = this.GetUser()!;
            if (!user.IsAdmin)
            {
                var approvedExamQuestionsIds = existingExamQuestions.Where(q => q.Exam.IsApproved).Select(q => q.Id).ToArray();
                return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "The following exam questions are already approved so they can't be updated.",
                            Data = new Dictionary<string, object> { ["ApprovedExamQuestions"] = approvedExamQuestionsIds }
                        });
            }
            _dbContext.ExamsQuestions.RemoveRange(existingExamQuestions);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [HttpPost]
        [LoggedInFilter]
        public async Task<IActionResult> Create([FromBody] CreateExamQuestionDto dto)
        {
            var examQuestion = new ExamQuestion
            {
                ExamId = dto.ExamId,
                Order = dto.Order,
                QuestionId = dto.QuestionId
            };
            await _dbContext.ExamsQuestions.AddAsync(examQuestion).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new { examQuestionsIds = new int[] { examQuestion.Id } }, _mapper.Map<ExamQuestionDto>(examQuestion));
        }
        [HttpPatch]
        [LoggedInFilter]
        public async Task<IActionResult> Update([FromBody] UpdateExamQuestionDto update)
        {
            var examQuestion = await _dbContext.ExamsQuestions.FindAsync(update.Id).ConfigureAwait(false);
            if(update.Order.HasValue)
            {
                examQuestion.Order = update.Order.Value;
            }
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}
