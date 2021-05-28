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
        private IQueryable<Question> GetPreparedQueryable(bool metadata = false)
        {
            var q = _dbContext.Questions
                .Include(q => q.Course)
                .AsQueryable();
            if (!metadata)
            {
                q = q.Include(e => e.Volunteer)
                    .Include(e => e.SubQuestions);
            }

            return q;
        }

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public QuestionController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Result is ordered by title ascending.</summary>
        /// <remarks>
        /// A user has access to:
        ///     1- All approved questions.
        ///     2- HIS not approved questions.
        /// An admin has access to:
        ///     All questions.
        /// </remarks>
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            var questions = _dbContext.Questions.AsQueryable();
            var user = this.GetUser();
            if (!(user?.IsAdmin ?? false))
            {
                var userId = user?.Id ?? -1;
                questions = questions.Where(q => q.IsApproved || q.VolunteerId == userId);
            }

            questions = questions.OrderBy(q => q.Title);
            return Ok(questions.Skip(info.Offset).Take(info.Count).Select(q => q.Id));
        }

        /// <param name="questionsIds">Ids of the questions to get.</param>
        /// <param name="metadata">Whether to return QuestionMetadataDto or QuestionDto.</param>
        /// <remarks>
        /// A user can get:
        ///     1- All approved questions.
        ///     2- HIS not approved questions.
        /// An admin can get:
        ///     All questions.
        /// </remarks>
        /// <response code="404">Ids of the non existing questions.</response>
        /// <response code="403">Ids of questions the user has no access rights to.</response>
        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<QuestionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<QuestionMetadataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Get([FromBody] int[] questionsIds, [FromQuery] bool metadata = false)
        {
            var questions = GetPreparedQueryable(metadata);
            var existingQuestions = questions.Where(e => questionsIds.Contains(e.Id));
            var nonExistingQuestions = questionsIds.Except(existingQuestions.Select(e => e.Id)).ToArray();
            if (nonExistingQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following questions don't exist.",
                        Data = new Dictionary<string, object>
                        {
                            ["NonExistingQuestions"] = nonExistingQuestions
                        }
                    });
            }

            var user = this.GetUser();
            if (!(user?.IsAdmin ?? false))
            {
                var notOwnedQuestions =
                   await existingQuestions.Where(e => e.VolunteerId != user.Id && !e.IsApproved)
                        .Select(q => q.Id)
                        .ToArrayAsync().ConfigureAwait(false);
                if (notOwnedQuestions.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following not approved questions.",
                            Data = new Dictionary<string, object> {["NotOwnedNotApprovedQuestions"] = notOwnedQuestions}
                        });
                }
            }

            if (metadata)
            {
                return Ok(_mapper.ProjectTo<QuestionMetadataDto>(existingQuestions));
            }

            return Ok(_mapper.ProjectTo<QuestionDto>(existingQuestions));
        }

        /// <summary>Deletes the specified questions.</summary>
        /// <param name="questionsIds">Ids of the questions to delete.</param>
        /// <remarks>
        /// A user can delete:
        ///     HIS NOT approved questions.
        /// An admin can delete:
        ///     All questions.
        /// </remarks>
        /// <response code="404">Ids of the non existing questions.</response>
        /// <response code="403">Ids of the questions user can't modify.</response>
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
                        Data = new Dictionary<string, object> {["NonExistingQuestions"] = nonExistingQuestions}
                    });
            }

            var user = this.GetUser()!;
            if (!user.IsAdmin)
            {
                var approvedOrNotOwnedQuestions = await existingQuestions
                    .Where(e => e.VolunteerId != user.Id || e.IsApproved)
                    .Select(q => q.Id)
                    .ToArrayAsync()
                    .ConfigureAwait(false);
                if (approvedOrNotOwnedQuestions.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following or they are approved.",
                            Data = new Dictionary<string, object>
                                {["NotOwnedOrApprovedQuestions"] = approvedOrNotOwnedQuestions}
                        });
                }
            }

            _dbContext.Questions.RemoveRange(existingQuestions);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>Creates a new question.</summary>
        /// <response code="201">Metadata of the newly created question.</response>
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
            return CreatedAtAction(nameof(Get), new {questionsIds = new int[] {question.Id}, metadata = true},
                _mapper.Map<QuestionMetadataDto>(question));
        }

        /// <summary>
        /// Returns questions that satisfy the filters ordered by title.
        /// </summary>
        /// <param name="filter">The filters to apply, null property means it won't be applied.</param>
        [HttpPost("Search")]
        [ProducesResponseType(typeof(IEnumerable<QuestionMetadataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<QuestionDto>), StatusCodes.Status200OK)]
        public IActionResult Search(QuestionSearchFilterDto filter)
        {
            IQueryable<Question> questions = GetPreparedQueryable(filter.Metadata);
            var user = this.GetUser();
            if (user == null)
            {
                filter.VolunteersIds = null;
            }

            if (!(user?.IsAdmin ?? false))
            {
                var userId = user?.Id ?? -1;
                questions = questions.Where(q => q.IsApproved || q.VolunteerId == userId);
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

        /// <summary>
        /// Updates a question.
        /// </summary>
        /// <param name="update">The update to apply, null fields mean no update to this property.</param>
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