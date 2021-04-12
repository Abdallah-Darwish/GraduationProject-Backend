using AutoMapper;
using GradProjectServer.Common;
using GradProjectServer.DTO;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Infrastructure.Programs;
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
    public class SubQuestionController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IProgramService _programService;
        public SubQuestionController(AppDbContext dbContext, IMapper mapper, IProgramService programService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _programService = programService;
        }
        public async Task<IActionResult> Get([FromBody] int[] subQuestionsIds, bool metadata = false)
        {
            var existingSubQuestions = _dbContext.SubQuestions.Where(e => subQuestionsIds.Contains(e.Id));
            var nonExistingSubQuestions = subQuestionsIds.Except(existingSubQuestions.Select(e => e.Id)).ToArray();
            if (nonExistingSubQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following questions don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingQuestions"] = nonExistingSubQuestions }
                        });
            }
            var user = this.GetUser();
            if (user?.IsAdmin ?? false)
            {
                var notOwnedSubQuestions = existingSubQuestions.Where(e => e.Question.VolunteerId != user.Id && !e.Question.IsApproved).ToArray();
                if (notOwnedSubQuestions.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User dosn't own the following not approved sub questions.",
                            Data = new Dictionary<string, object> { ["NotOwnedNotApprovedSubQuestions"] = notOwnedSubQuestions }
                        });
                }
            }
            var result = new List<SubQuestion>();
            foreach (var q in existingSubQuestions)
            {
                switch (q.Type)
                {
                    case SubQuestionType.MultipleChoice:
                        result.Add(await _dbContext.MCQSubQuestions.FindAsync(q.Id).ConfigureAwait(false));
                        break;
                    case SubQuestionType.Blank:
                        result.Add(await _dbContext.BlankSubQuestions.FindAsync(q.Id).ConfigureAwait(false));
                        break;
                    case SubQuestionType.Programming:
                        result.Add(await _dbContext.ProgrammingSubQuestions.FindAsync(q.Id).ConfigureAwait(false));
                        break;
                    default:
                        break;
                }
            }
            if (metadata)
            {
                return Ok(_mapper.Map<List<SubQuestionMetadataDto>>(existingSubQuestions));
            }
            return Ok(_mapper.Map<List<SubQuestionDto>>(existingSubQuestions));

        }
        public async Task<IActionResult> Create([FromBody] CreateSubQuestionDto dto)
        {
            SubQuestion subQuestion;
            switch (dto)
            {
                case CreateBlankSubQuestionDto blank:
                    BlankSubQuestion bq = new()
                    {
                        Answer = blank.Answer,
                        Checker = blank.Checker != null ? await _programService.SaveProgram(blank.Checker) : null
                    };
                    subQuestion = bq;
                    await _dbContext.BlankSubQuestions.AddAsync(bq).ConfigureAwait(false);
                    break;

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
                            SubQuestion = mq
                        }).ToArray();
                    subQuestion = mq;
                    await _dbContext.MCQSubQuestions.AddAsync(mq).ConfigureAwait(false);
                    break;
                case CreateProgrammingSubQuestionDto pro:
                    ProgrammingSubQuestion pq = new()
                    {
                        Checker = await _programService.SaveProgram(pro.Checker)
                    };
                    subQuestion = pq;
                    await _dbContext.ProgrammingSubQuestions.AddAsync(pq).ConfigureAwait(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dto), "The dto isn't known.");
            }
            subQuestion.Content = dto.Content;
            subQuestion.Type = dto.Type;
            subQuestion.Tags = dto.Tags?.Select(t => new SubQuestionTag { TagId = t, SubQuestion = subQuestion }).ToArray() ?? Array.Empty<SubQuestionTag>();
            subQuestion.QuestionId = dto.QuestionId;
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new { subQuestionsIds = new int[] { subQuestion.Id }, metadata = false }, subQuestion);
        }
        [LoggedInFilter]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] int[] subQuestionsIds)
        {
            var existingSubQuestions = _dbContext.SubQuestions.Where(e => subQuestionsIds.Contains(e.Id));
            var nonExistingSubQuestions = subQuestionsIds.Except(existingSubQuestions.Select(e => e.Id)).ToArray();
            if (nonExistingSubQuestions.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following sub questions don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingSubQuestions"] = nonExistingSubQuestions }
                        });
            }
            var user = this.GetUser()!;
            if (!user.IsAdmin)
            {
                var approvedOrNotOwnedSubQuestions = existingSubQuestions.Where(e => e.Question.VolunteerId != user.Id || e.Question.IsApproved).ToArray();
                if (approvedOrNotOwnedSubQuestions.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User dosn't own the following sub questions or they are approved.",
                            Data = new Dictionary<string, object> { ["NotOwnedOrApprovedSubQuestions"] = approvedOrNotOwnedSubQuestions }
                        });
                }
            }
            _dbContext.SubQuestions.RemoveRange(existingSubQuestions);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [HttpPatch]
        [LoggedInFilter]
        public async Task<IActionResult> Update([FromBody] UpdateSubQuestionDto update)
        {
            SubQuestion baseSubQuestion;
            switch (update)
            {
                case UpdateBlankSubQuestionDto blankUpdate:
                    var bq = await _dbContext.BlankSubQuestions.FindAsync(update.Id).ConfigureAwait(false);
                    baseSubQuestion = bq;
                    if (blankUpdate.UpdateAnswer)
                    {
                        bq.Answer = blankUpdate.Answer;
                    }
                    if (blankUpdate.UpdateChecker)
                    {
                        bq.Checker = blankUpdate.Checker == null ?
                            null :
                            await _programService.SaveProgram(blankUpdate.Checker, $"BlankSubQuestion{blankUpdate.Id}Checker").ConfigureAwait(false);
                    }

                    break;
                case UpdateMCQSubQuestionDto mcqUpdate:
                    var mcq = await _dbContext.MCQSubQuestions.FindAsync(update.Id).ConfigureAwait(false);
                    baseSubQuestion = mcq;

                    if (mcqUpdate.IsCheckBox.HasValue)
                    {
                        mcq.IsCheckBox = mcqUpdate.IsCheckBox.Value;
                    }
                    if ((mcqUpdate.ChoicesToAdd?.Length ?? 0) > 0)
                    {
                        foreach (var newChoice in mcqUpdate.ChoicesToAdd!)
                        {
                            mcq.Choices.Add(new MCQSubQuestionChoice
                            {
                                Content = newChoice.Content,
                                Weight = newChoice.Weight,
                                SubQuestion = mcq,
                                SubQuestionId = mcq.Id
                            });
                        }
                    }
                    if ((mcqUpdate.ChoicesToDelete?.Length ?? 0) > 0)
                    {
                        var choicesToRemove = mcq.Choices.Where(c => mcqUpdate.ChoicesToDelete!.Contains(c.Id));
                        foreach (var choiceToRemove in choicesToRemove)
                        {
                            mcq.Choices.Remove(choiceToRemove);
                        }
                    }
                    if ((mcqUpdate.ChoicesToUpdate?.Length ?? 0) > 0)
                    {
                        var choicesToUpdate = mcqUpdate.ChoicesToUpdate!.ToDictionary(c => c.Id);
                        foreach (var choice in mcq.Choices)
                        {
                            if (!choicesToUpdate.TryGetValue(choice.Id, out var choiceUpdate)) { continue; }
                            if (choiceUpdate.Content != null)
                            {
                                choice.Content = choiceUpdate.Content;
                            }
                            if (choiceUpdate.Weight.HasValue)
                            {
                                choice.Weight = choiceUpdate.Weight.Value;
                            }
                        }
                    }
                    break;
                case UpdateProgrammingSubQuestionDto proUpdate:
                    var pro = await _dbContext.ProgrammingSubQuestions.FindAsync(update.Id).ConfigureAwait(false);
                    baseSubQuestion = pro;
                    if (proUpdate.Checker != null)
                    {
                        pro.Checker = await _programService.SaveProgram(proUpdate.Checker, $"ProgrammingSubQuestion{pro.Id}Checker").ConfigureAwait(false);

                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(update), update, "Update type is not supported.");
            }
            if (update.Content != null)
            {
                baseSubQuestion.Content = update.Content;
            }
            if (update.QuestionId.HasValue)
            {
                baseSubQuestion.QuestionId = update.QuestionId.Value;
            }
            if ((update.TagsToAdd?.Length ?? 0) > 0)
            {
                await _dbContext.SubQuestionsTags
                    .AddRangeAsync(update.TagsToAdd!.Select(t => new SubQuestionTag { SubQustionId = baseSubQuestion.Id, TagId = t })).ConfigureAwait(false);
            }
            if ((update.TagsToDelete?.Length ?? 0) > 0)
            {
                var tagsToDelete = baseSubQuestion.Tags.Where(t => update.TagsToDelete!.Contains(t.TagId));
                foreach (var tag in tagsToDelete)
                {
                    baseSubQuestion.Tags.Remove(tag);
                }
            }
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}
