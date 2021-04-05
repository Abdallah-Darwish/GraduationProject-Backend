using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Questions;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Infrastructure;
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

        public async Task<ActionResult<QuestionDto[]>> GetMetadata([FromBody] int[] questionsIds)
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
            return Ok(await _mapper.ProjectTo<QuestionMetadataDto>(existingQuestions).ToArrayAsync().ConfigureAwait(false));
        }
        [LoggedInFilter]
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
            return Ok(await _mapper.ProjectTo<QuestionDto>(existingQuestions).ToArrayAsync().ConfigureAwait(false));
        }
        //todo: shouldn't this be in FluentMapper ?
        private SubQuestion SubQustionDtoToEntity(Question container, CreateSubQuestionDto dto)
        {
            void FillBase(SubQuestion q)
            {
                q.Content = dto.Content;
                q.Type = dto.Type;
                q.Tags = dto.Tags?.Select(t => new SubQuestionTag { TagId = t, SubQuestion = q }).ToArray() ?? Array.Empty<SubQuestionTag>();
                q.Question = container;
            }
            switch (dto)
            {
                case CreateBlankSubQuestionDto blank:
                    BlankSubQuestion bq = new()
                    {
                        Answer = blank.Answer,
                        Checker = _mapper.Map<Program>(blank.Checker)
                    };
                    FillBase(bq);
                    return bq;

                case CreateMCQSubQuestionDto mcq:
                    MCQSubQuestion mq = new()
                    {
                        IsCheckBox = mcq.IsCheckBox,
                    };
                    mq.Choices = mcq.Choices
                        .Select(c => new MCQSubQuestionChoice
                        {
                            Content = c.Content,
                            Weight = c.Weight,
                            Question = mq
                        }).ToArray();
                    FillBase(mq);
                    return mq;
                case CreateProgrammingSubQuestionDto pro:
                    ProgrammingSubQuestion pq = new()
                    {
                        Checker = _mapper.Map<Program>(pro.Checker)
                    };
                    FillBase(pq);
                    return pq;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dto), "The dto isn't known.");
            }
        }
        [LoggedInFilter]
        public async Task<ActionResult<int>> Create(CreateQuestionDto info)
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
            question.SubQuestions = info.SubQuestions.Select(sq => SubQustionDtoToEntity(question, sq)).ToArray();
            await _dbContext.Questions.AddAsync(question).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok(question.Id);
        }
        /// <summary>
        /// result is ordered by the id.
        /// </summary>
        public async Task<ActionResult<QuestionMetadataDto[]>> Search(QuestionSearchFilterDto filter)
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
            var result = questions.OrderByDescending(e => e.Id).Skip(filter.Offset).Take(filter.Count);
            QuestionMetadataDto[] resultDto = await _mapper.ProjectTo<QuestionMetadataDto>(result).ToArrayAsync().ConfigureAwait(false);
            return resultDto;
        }
        [LoggedInFilter]
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
            if ((update.SubQuestionsToDelete?.Length ?? 0) > 0)
            {
                var questionsToDelete = question.SubQuestions.Where(q => update.SubQuestionsToDelete!.Contains(q.Id));
                _dbContext.SubQuestions.RemoveRange(questionsToDelete);
            }
            if ((update.SubQuestionsToAdd?.Length ?? 0) > 0)
            {
                var newQuestions = update.SubQuestionsToAdd!
                    .Select(q => SubQustionDtoToEntity(question, q));
                await _dbContext.SubQuestions.AddRangeAsync(newQuestions).ConfigureAwait(false);
            }
            return Ok();
        }

    }
}
