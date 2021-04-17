using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Questions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public QuestionController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            return Ok(_dbContext.Questions.Skip(info.Offset).Take(info.Count).Select(q => q.Id));
        }

        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<QuestionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<QuestionMetadataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public IActionResult Get([FromBody] int[] questionsIds, [FromQuery] bool metadata = false)
        {
            var existingQuestions = _dbContext.Questions.Where(e => questionsIds.Contains(e.Id));
            var nonExistingQuestions = questionsIds.Except(existingQuestions.Select(e => e.Id)).ToArray();
            if (nonExistingQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following questions don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingQuestions"] = nonExistingQuestions }
                        });
            }
            var user = this.GetUser();
            if (user?.IsAdmin ?? false)
            {
                var notOwnedQuestions = existingQuestions.Where(e => e.VolunteerId != user.Id && !e.IsApproved).ToArray();
                if (notOwnedQuestions.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User dosn't own the following not approved questions.",
                            Data = new Dictionary<string, object> { ["NotOwnedNotApprovedQuestions"] = notOwnedQuestions }
                        });
                }
            }
            if (metadata)
            {
                return Ok(_mapper.ProjectTo<QuestionMetadataDto>(existingQuestions));
            }
            return Ok(_mapper.ProjectTo<QuestionDto>(existingQuestions));
        }

        [LoggedInFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete([FromBody] int[] questionsIds)
        {
            var existingQuestions = _dbContext.Questions.Where(e => questionsIds.Contains(e.Id));
            var nonExistingQuestions = questionsIds.Except(existingQuestions.Select(e => e.Id)).ToArray();
            if (nonExistingQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following questions don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingQuestions"] = nonExistingQuestions }
                        });
            }
            var user = this.GetUser()!;
            if (!user.IsAdmin)
            {
                var approvedOrNotOwnedQuestions = existingQuestions.Where(e => e.VolunteerId != user.Id || e.IsApproved).ToArray();
                if (approvedOrNotOwnedQuestions.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User dosn't own the following or they are approved.",
                            Data = new Dictionary<string, object> { ["NotOwnedOrApprovedQuestions"] = approvedOrNotOwnedQuestions }
                        });
                }
            }
            _dbContext.Questions.RemoveRange(existingQuestions);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [LoggedInFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(QuestionMetadataDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<QuestionMetadataDto>> Create(CreateQuestionDto info)
        {
            var user = this.GetUser()!;
            var question = new Question
            {
                Content = info.Content,
                Title = info.Title,
                CourseId = info.CourseId,
                IsApproved = false,
                VolunteerId = user.Id,
            };
            question.SubQuestions = new List<SubQuestion>();
            await _dbContext.Questions.AddAsync(question).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new { questionsIds = new int[] { question.Id }, metadata = true }, _mapper.Map<QuestionMetadataDto>(question));
        }
        /// <summary>
        /// result is ordered by the title.
        /// </summary>
        [HttpPost("Search")]
        [ProducesResponseType(typeof(IEnumerable<QuestionMetadataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<QuestionDto>), StatusCodes.Status200OK)]
        public IActionResult Search(QuestionSearchFilterDto filter)
        {
            var questions = _dbContext.Questions.AsQueryable();
            var user = this.GetUser();
            if (user == null) { filter.VolunteersIds = null; }
            else if (!user.IsAdmin && (filter.VolunteersIds?.Length ?? 0) > 0)
            {
                filter.VolunteersIds = new int[] { user.Id };
            }
            if (filter.TitleMask != null)
            {
                questions = questions.Where(e => EF.Functions.Like(e.Title, filter.TitleMask));
            }
            if ((filter.QuestionsIds?.Length ?? 0) > 0)
            {
                questions = questions.Where(e => filter.QuestionsIds!.Contains(e.Id));
            }
            if ((filter.CoursesIds?.Length ?? 0) > 0)
            {
                questions = questions.Where(e => filter.CoursesIds!.Contains(e.CourseId));
            }
            if ((filter.TagsIds?.Length ?? 0) > 0)
            {
                //probably very bad and I have to write the sql manually
                questions = questions.Where(e => !filter.TagsIds!.Except(e.Tags.Select(e => e.Id)).Any());
            }
            if (filter.IsApproved.HasValue)
            {
                questions = questions.Where(e => e.IsApproved == filter.IsApproved.Value);
            }
            if ((filter.VolunteersIds?.Length ?? 0) > 0)
            {
                questions = questions.Where(e => filter.VolunteersIds!.Contains(e.VolunteerId));
            }
            var result = questions.OrderBy(e => e.Title).Skip(filter.Offset).Take(filter.Count);
            if (filter.Metadata)
            {
                return Ok(_mapper.ProjectTo<QuestionMetadataDto>(result));
            }
            return Ok(_mapper.ProjectTo<QuestionDto>(result));
        }
        [LoggedInFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateQuestionDto update)
        {
            var question = await _dbContext.Questions
                .FindAsync(update.QuestionId)
                .ConfigureAwait(false);
            if (update.Content != null)
            {
                question.Content = update.Content;
            }
            if (update.CourseId.HasValue)
            {
                question.CourseId = update.CourseId.Value;
            }
            if (update.Title != null)
            {
                question.Title = update.Title;
            }
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

    }
}
