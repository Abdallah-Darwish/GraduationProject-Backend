using AutoMapper;
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
    //todo: maybe we should move all of the logic into seperate controllers and add custom middlewares that would translate custom exceptions into status codes
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

        /// <remarks>
        /// Result is ordered by year descending then name ascending.
        /// A user has access to:
        ///     1- All approved exams.
        ///     2- HIS not approved exams.
        /// An admin has access to:
        ///     All exams.
        /// </remarks>
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            var user = this.GetUser();
            var exams = _dbContext.Exams.AsQueryable();
            if (!(user?.IsAdmin ?? false))
            {
                var userId = user?.Id ?? -1;
                exams = exams.Where(e => e.VolunteerId == userId || e.IsApproved);
            }

            exams = exams.OrderByDescending(e => e.Year).ThenBy(e => e.Name);
            return Ok(exams.Skip(info.Offset).Take(info.Count).Select(e => e.Id));
        }

        /// <param name="examsIds">Ids of the exams to get.</param>
        /// <param name="metadata">Whether to return ExamMetadataDto or ExamDto.</param>
        /// <remarks>
        /// A user can get:
        ///     1- All approved exams.
        ///     2- HIS not approved exams.
        /// An admin can get:
        ///     All exams.
        /// </remarks>
        /// <response code="404">Ids of the non existing exams.</response>
        /// <response code="403">Ids of exams the user has no access rights to.</response>
        [HttpPost("Get")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<ExamMetadataDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<ExamDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        public IActionResult Get([FromBody] int[] examsIds, bool metadata = false)
        {
            var existingExams = _dbContext.Exams.Where(e => examsIds.Contains(e.Id));
            var nonExistingExams = examsIds.Except(existingExams.Select(e => e.Id)).ToArray();
            if (nonExistingExams.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following exams don't exist.",
                        Data = new Dictionary<string, object> {["NonExistingExams"] = nonExistingExams}
                    });
            }

            var user = this.GetUser();
            if (!(user?.IsAdmin ?? false))
            {
                var userId = user?.Id ?? -1;
                var notOwnedExams = existingExams.Where(e => e.VolunteerId != userId && !e.IsApproved).ToArray();
                if (notOwnedExams.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following not approved exams.",
                            Data = new Dictionary<string, object> {["NotOwnedNotApprovedExams"] = notOwnedExams}
                        });
                }
            }

            if (metadata)
            {
                return Ok(_mapper.ProjectTo<ExamMetadataDto>(existingExams));
            }

            return Ok(_mapper.ProjectTo<ExamDto>(existingExams));
        }

        /// <summary>
        /// Returns exams that satisfy the filters ordered by year descending then by Name ascending.
        /// </summary>
        /// <param name="filter">The filters to apply, null property means it won't be applied.</param>
        [HttpPost("Search")]
        [ProducesResponseType(typeof(IEnumerable<ExamDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<ExamMetadataDto>), StatusCodes.Status200OK)]
        public IActionResult Search([FromBody] ExamSearchFilterDto filter)
        {
            var exams = _dbContext.Exams.AsQueryable();
            var user = this.GetUser();
            if (user == null)
            {
                filter.VolunteersIds = null;
            }

            if (!(user?.IsAdmin ?? false))
            {
                var userId = user?.Id ?? -1;
                exams = exams.Where(e => e.IsApproved || e.VolunteerId == userId);
            }

            if (filter.NameMask != null)
            {
                exams = exams.Where(e => EF.Functions.Like(e.Name, filter.NameMask));
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

            exams = exams.OrderByDescending(e => e.Year).ThenBy(e => e.Name);
            var result = exams.Skip(filter.Offset).Take(filter.Count);
            if (filter.Metadata)
            {
                return Ok(_mapper.ProjectTo<ExamMetadataDto>(result));
            }

            return Ok(_mapper.ProjectTo<ExamDto>(result));
        }

        /// <summary>Deletes the specified exams.</summary>
        /// <param name="examsIds">Ids of the exams to delete.</param>
        /// <remarks>
        /// A user can delete:
        ///     HIS NOT approved exams.
        /// An admin can delete:
        ///     All exams.
        /// </remarks>
        /// <response code="404">Ids of the non existing exams.</response>
        /// <response code="403">Ids of the exams user can't modify.</response>
        [LoggedInFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status403Forbidden)]
        //todo: create not null attribute
        //todo: create min/max length attribute
        //todo: create distinct attribute
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
                        Data = new Dictionary<string, object> {["NonExistingExams"] = nonExistingExams}
                    });
            }

            var user = this.GetUser()!;
            if (!user.IsAdmin)
            {
                var notOwnedExams = existingExams
                    .Where(e => e.VolunteerId != user.Id)
                    .Select(e => e.Id)
                    .ToArray();
                if (notOwnedExams.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "User doesn't own the following exams so he can't modify them.",
                            Data = new Dictionary<string, object> {["NotOwnedExams"] = notOwnedExams}
                        });
                }

                var approvedExams = existingExams
                    .Where(e => e.IsApproved)
                    .Select(e => e.Id)
                    .ToArray();
                if (approvedExams.Length > 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorDTO
                        {
                            Description = "The following exams are already approved so the user can't modify them.",
                            Data = new Dictionary<string, object> {["ApprovedExams"] = approvedExams}
                        });
                }
            }

            _dbContext.Exams.RemoveRange(existingExams);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>Creates a new exam.</summary>
        /// <response code="201">Metadata of the newly created exam.</response>
        [LoggedInFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ExamMetadataDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<ExamMetadataDto>> Create([FromBody] CreateExamDto data)
        {
            var user = this.GetUser()!;
            var exam = new Exam
            {
                CourseId = data.CourseId,
                Duration = data.Duration,
                IsApproved = false,
                Name = data.Name,
                Semester = data.Semester,
                Type = data.Type,
                VolunteerId = user.Id,
                Year = data.Year,
            };
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new {examsIds = new int[] {exam.Id}, metadata = true},
                _mapper.Map<ExamMetadataDto>(exam));
        }

        /// <summary>
        /// Updates an exam.
        /// </summary>
        /// <param name="update">The update to apply, null fields mean no update to this property.</param>
        [LoggedInFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateExamDto update)
        {
            var exam = await _dbContext.Exams.FindAsync(update.ExamId).ConfigureAwait(false);
            var user = this.GetUser()!;


            if (update.CourseId.HasValue)
            {
                exam.CourseId = update.CourseId.Value;
            }

            if (update.Duration.HasValue)
            {
                exam.Duration = update.Duration.Value;
            }

            if (!string.IsNullOrWhiteSpace(update.Name))
            {
                exam.Name = update.Name;
            }

            if (update.Semester.HasValue)
            {
                exam.Semester = update.Semester.Value;
            }

            if (update.Type.HasValue)
            {
                exam.Type = update.Type.Value;
            }

            if (update.Year.HasValue)
            {
                exam.Year = update.Year.Value;
            }

            //todo: log if user is not admin and it has value
            if (user.IsAdmin && update.IsApproved.HasValue)
            {
                exam.IsApproved = update.IsApproved.Value;
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}