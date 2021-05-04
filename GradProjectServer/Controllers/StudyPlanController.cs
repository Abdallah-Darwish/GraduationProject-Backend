using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Tags;
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
using GradProjectServer.DTO.StudyPlans;
using GradProjectServer.Services.Infrastructure;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudyPlanController : ControllerBase
    {
        private IQueryable<StudyPlan> GetPreparedQueryable(bool metadata = false)
        {
            var q = _dbContext.StudyPlans
                .Include(s => s.Major)
                .AsQueryable();
            if (!metadata)
            {
                q = q.Include(s => s.CourseCategories)
                    .ThenInclude(s => s.Category)
                    .Include(s => s.CourseCategories)
                    .ThenInclude(s => s.Courses)
                    .ThenInclude(s => s.Course)
                    .Include(s => s.CourseCategories)
                    .ThenInclude(s => s.Courses)
                    .ThenInclude(s => s.Prerequisites)
                    .Include(s => s.CourseCategories)
                    .ThenInclude(s => s.Courses)
                    .ThenInclude(s => s.Prerequisites)
                    .ThenInclude(s => s.Course);
            }

            return q;
        }

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public StudyPlanController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <reamarks>Result is ordered by major name then year.</reamarks>
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            return Ok(_dbContext.StudyPlans
                .OrderBy(s => s.Major.Name)
                .ThenBy(s => s.Year)
                .Skip(info.Offset)
                .Take(info.Count)
                .Select(t => t.Id));
        }

        /// <param name="studyPlansIds">Ids of the study plans to get.</param>
        /// <param name="metadata">Whether to return StudyPlanDto or StudyPlanMetadataDto.</param>
        /// <response code="404">Ids of the non existing study plans.</response>
        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<StudyPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<StudyPlanMetadataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public IActionResult Get([FromBody] int[] studyPlansIds, [FromQuery] bool metadata = false)
        {
            var studyPlans = GetPreparedQueryable(metadata);
            var existingStudyPlans = studyPlans.Where(c => studyPlansIds.Contains(c.Id));
            var nonExistingStudyPlans = studyPlansIds.Except(existingStudyPlans.Select(c => c.Id)).ToArray();
            if (nonExistingStudyPlans.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following study plans don't exist.",
                        Data = new Dictionary<string, object> {["NonExistingStuyPlans"] = nonExistingStudyPlans}
                    });
            }

            if (metadata)
            {
                return Ok(_mapper.ProjectTo<StudyPlanMetadataDto>(existingStudyPlans));
            }

            return Ok(_mapper.ProjectTo<StudyPlanDto>(existingStudyPlans));
        }

        /// <summary>
        /// Creates a new study plan.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <reponse code="201">The newly created study plan metadata.</reponse>
        [AdminFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(StudyPlanMetadataDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<StudyPlanMetadataDto>> Create([FromBody] CreateStudyPlanDto info)
        {
            var newStudyPlan = new StudyPlan
            {
                MajorId = info.MajorId,
                Year = info.Year,
            };
            await _dbContext.StudyPlans.AddAsync(newStudyPlan).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new {tagsIds = new int[] {newStudyPlan.Id}},
                _mapper.Map<StudyPlanMetadataDto>(newStudyPlan));
        }

        /// <summary>
        /// Updates a study plan.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <param name="update">The update to apply, null fields mean no update to this property.</param>
        [AdminFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateStudyPlanDto update)
        {
            var studyPlan = await _dbContext.StudyPlans.FindAsync(update.Id).ConfigureAwait(false);
            if (update.Year != null)
            {
                studyPlan.Year = update.Year.Value;
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Deletes the specified study plans.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <param name="studyPlansIds">Ids of the study plans to delete.</param>
        /// <response code="404">Ids of the non existing study plans.</response>
        [AdminFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] int[] studyPlansIds)
        {
            var existingStudyPlans = _dbContext.StudyPlans.Where(c => studyPlansIds.Contains(c.Id));
            var nonExistingStudyPlans = studyPlansIds.Except(existingStudyPlans.Select(c => c.Id)).ToArray();
            if (nonExistingStudyPlans.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following study plans don't exist.",
                        Data = new Dictionary<string, object> {["NonExistingStudyPlans"] = nonExistingStudyPlans}
                    });
            }

            _dbContext.StudyPlans.RemoveRange(existingStudyPlans);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}