using AutoMapper;
using GradProjectServer.Common;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Exams;
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
    public class ExamController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public ExamController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        /// <summary>
        /// A user can get:
        ///     1- All approved exams.
        ///     2- HIS not approved exams.
        /// An admin can get:
        ///     All exams.
        /// </summary>
        /// <param name="examsIds">Ids of the exams to get.</param>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ExamMetadataDto[]>> GetMetadata([FromBody] int[] examsIds)
        {
            var existingExams = _dbContext.Exams.Where(e => examsIds.Contains(e.Id));
            var nonExistingExams = examsIds.Except(existingExams.Select(e => e.Id)).ToArray();
            if (nonExistingExams.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following exams don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingExams"] = nonExistingExams }
                        });
            }
            var user = this.GetUser()!;
            if (!user.IsAdmin)
            {
                var notOwnedExams = existingExams.Where(e => e.VolunteerId != user.Id && !e.IsApproved).ToArray();
                if (notOwnedExams.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following not approved exams.",
                            Data = new Dictionary<string, object> { ["NotOwnedNotApprovedExams"] = notOwnedExams }
                        });
                }
            }
            return Ok(await _mapper.ProjectTo<ExamMetadataDto>(existingExams).ToArrayAsync().ConfigureAwait(false));

        }
        /// <summary>
        /// Result is orderd descending by the year 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ExamMetadataDto[]), StatusCodes.Status200OK)]
        public async Task<ActionResult<ExamMetadataDto[]>> Search([FromBody] ExamSearchFilterDto filter)
        {
            var exams = _dbContext.Exams.AsQueryable();
            var user = this.GetUser();
            if (user == null) { filter.VolunteersIds = null; }
            else if (!user.IsAdmin && (filter.VolunteersIds?.Length ?? 0) > 0)
            {
                filter.VolunteersIds = new int[] { user.Id };
            }
            if (filter.NameMask != null)
            {
                exams = exams.Where(e => EF.Functions.Like(e.Name, filter.NameMask));
            }
            if ((filter.Ids?.Length ?? 0) > 0)
            {
                exams = exams.Where(e => filter.Ids!.Contains(e.Id));
            }
            if ((filter.Courses?.Length ?? 0) > 0)
            {
                exams = exams.Where(e => filter.Courses!.Contains(e.CourseId));
            }
            if (filter.MinDuration != null)
            {
                exams = exams.Where(e => e.Duration >= filter.MinDuration);
            }
            if (filter.MaxDuration != null)
            {
                exams = exams.Where(e => e.Duration <= filter.MaxDuration);
            }
            if (filter.MinYear != null)
            {
                exams = exams.Where(e => e.Year >= filter.MinYear);
            }
            if (filter.MaxYear != null)
            {
                exams = exams.Where(e => e.Year <= filter.MaxYear);
            }
            if ((filter.Semesters?.Length ?? 0) > 0)
            {
                exams = exams.Where(e => filter.Semesters!.Contains(e.Semester));
            }
            if ((filter.Types?.Length ?? 0) > 0)
            {
                exams = exams.Where(e => filter.Types!.Contains(e.Type));
            }
            if ((filter.Tags?.Length ?? 0) > 0)
            {
                //probably very bad and I have to write the sql manually
                exams = exams.Where(e => !filter.Tags!.Except(e.Tags.Select(e => e.Id)).Any());
            }
            if (filter.IsApproved.HasValue)
            {
                exams = exams.Where(e => e.IsApproved == filter.IsApproved.Value);
            }
            if ((filter.VolunteersIds?.Length ?? 0) > 0)
            {
                exams = exams.Where(e => filter.VolunteersIds!.Contains(e.VolunteerId));
            }
            var result = exams.OrderByDescending(e => e.Year).Skip(filter.Offset).Take(filter.Count);
            ExamMetadataDto[] resultDto = await _mapper.ProjectTo<ExamMetadataDto>(result).ToArrayAsync().ConfigureAwait(false);
            return resultDto;
        }
        /// <summary>
        /// Deletes the specified exams.
        /// A user can delete:
        ///     HIS not approved exams.
        /// An admin can delete:
        ///     All exams.
        /// </summary>
        /// <param name="examsIds">Ids of the exams to delete</param>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //todo: create not null attribute
        //todo: create min/max length attribute
        public async Task<IActionResult> Delete([FromBody] int[] examsIds)
        {
            var existingExams = _dbContext.Exams.Where(e => examsIds.Contains(e.Id));
            var nonExistingExams = examsIds.Except(existingExams.Select(e => e.Id)).ToArray();
            if (nonExistingExams.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following exams don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingExams"] = nonExistingExams }
                        });
            }
            var user = this.GetUser()!;
            if (!user.IsAdmin)
            {
                var notOwnedExams = existingExams.Where(e => e.VolunteerId != user.Id).ToArray();
                if (notOwnedExams.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following exams so he can't modify them.",
                            Data = new Dictionary<string, object> { ["NotOwnedExams"] = notOwnedExams }
                        });
                }
                var approvedExams = existingExams.Where(e => e.IsApproved).ToArray();
                if (approvedExams.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                       new ErrorDTO
                       {
                           Description = "The following exams are already approved so the user can't modify them.",
                           Data = new Dictionary<string, object> { ["ApprovedExams"] = approvedExams }
                       });
                }
            }
            await _dbContext.Database.ExecuteSqlRawAsync($@"DELETE FROM {nameof(Exam)} WHERE Id IN @examsIds", examsIds).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Create([FromBody] CreateExamDto data)
        {
            var user = this.GetUser()!;
            var exam = new Exam
            {
                CourseId = (await _dbContext.SubQuestions.FindAsync(data.SubQuestions[0].QuestionId).ConfigureAwait(false)).Question.CourseId,
                Duration = data.Duration,
                IsApproved = false,
                Name = data.Name,
                Semester = data.Semester,
                Type = data.Type,
                VolunteerId = user.Id,
                Year = data.Year,
            };
            //todo: TESSSSSSSSST
            exam.SubQuestions = data.SubQuestions
                .Select(q =>
                new ExamSubQuestion
                {
                    Exam = exam,
                    SubQuestionId = q.QuestionId,
                    Weight = q.Weight
                })
                .ToArray();
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok(exam.Id);
        }
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update([FromBody] UpdateExamDto update)
        {
            var exam = await _dbContext.Exams.FindAsync(update.ExamId).ConfigureAwait(false);
            var user = this.GetUser()!;


            if (update.CourseId.HasValue) { exam.CourseId = update.CourseId.Value; }
            if (update.Duration.HasValue) { exam.Duration = update.Duration.Value; }
            if (!string.IsNullOrWhiteSpace(update.Name)) { exam.Name = update.Name; }
            if (update.Semester.HasValue) { exam.Semester = update.Semester.Value; }
            if (update.Type.HasValue) { exam.Type = update.Type.Value; }
            if (update.Year.HasValue) { exam.Year = update.Year.Value; }
            //todo: log if user is not admin and it has value
            if (user.IsAdmin && update.IsApproved.HasValue) { exam.IsApproved = update.IsApproved.Value; }
            if ((update.SubQuestionsToAdd?.Length ?? 0) > 0)
            {
                var subQuestionsToAdd = update.SubQuestionsToAdd!
                    .Select(q =>
                    new ExamSubQuestion
                    {
                        ExamId = exam.Id,
                        SubQuestionId = q.QuestionId,
                        Weight = q.Weight
                    });
                _dbContext.ExamsSubQuestions.AddRange(subQuestionsToAdd);
            }
            if ((update.SubQuestionsToDelete?.Length ?? 0) > 0)
            {
                var subQuestionsToDelete = exam.SubQuestions
                    .Where(q => update.SubQuestionsToDelete!.Contains(q.SubQuestionId));
                _dbContext.ExamsSubQuestions.RemoveRange(subQuestionsToDelete);
            }
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}
