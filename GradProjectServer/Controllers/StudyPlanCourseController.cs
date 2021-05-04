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
using GradProjectServer.DTO.CourseCategories;
using GradProjectServer.DTO.StudyPlanCourseCategories;
using GradProjectServer.DTO.StudyPlanCourses;
using GradProjectServer.DTO.StudyPlans;
using GradProjectServer.Services.Infrastructure;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudyPlanCourseController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public StudyPlanCourseController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new study plan course.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <reponse code="201">The newly created study plan course.</reponse>
        [AdminFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(StudyPlanCourseDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<StudyPlanCourseDto>> Create([FromBody] CreateStudyPlanCourseDto info)
        {
            StudyPlanCourse newStudyPlanCourse = new()
            {
                CategoryId = info.StudyPlanCourseCategoryId,
                CourseId = info.CourseId,
            };
            await _dbContext.StudyPlansCourses.AddAsync(newStudyPlanCourse).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            var prerequisites = info.Prerequisites.Select(p => new StudyPlanCoursePrerequisite
            {
                CourseId = newStudyPlanCourse.Id,
                PrerequisiteId = p
            });
            await _dbContext.StudyPlansCoursesPrerequisites.AddRangeAsync(prerequisites).ConfigureAwait(false);
            return StatusCode(StatusCodes.Status201Created, _mapper.Map<StudyPlanCourseDto>(newStudyPlanCourse));
        }

        /// <summary>
        /// Updates a study plan course.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <param name="update">The update to apply, null fields mean no update to this property.</param>
        [AdminFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateStudyPlanCourseDto update)
        {
            var studyPlanCourse =
                await _dbContext.StudyPlansCourses.FindAsync(update.Id).ConfigureAwait(false);
            if (update.PrerequisitesToDelete != null)
            {
                var prerequisitesToDeleteSet = update.PrerequisitesToDelete.ToHashSet();
                foreach (var pre in studyPlanCourse.Prerequisites)
                {
                    if (!prerequisitesToDeleteSet.Contains(pre.CourseId))
                    {
                        continue;
                    }

                    studyPlanCourse.Prerequisites.Remove(pre);
                }
            }

            if (update.PrerequisitesToAdd != null)
            {
                var prerequisites = update.PrerequisitesToAdd.Select(p => new StudyPlanCoursePrerequisite
                {
                    CourseId = update.Id,
                    PrerequisiteId = p
                });
                await _dbContext.StudyPlansCoursesPrerequisites.AddRangeAsync(prerequisites).ConfigureAwait(false);
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Deletes the specified study plan course.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <param name="studyPlanCourseIds">Ids of the study plan course to delete.</param>
        /// <response code="404">Ids of the non existing study plan course.</response>
        [AdminFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] int[] studyPlanCourseIds)
        {
            var existingStudyPlanCourses =
                _dbContext.StudyPlansCourses.Where(c => studyPlanCourseIds.Contains(c.Id));
            var nonExistingStudyPlanCourses =
                studyPlanCourseIds.Except(existingStudyPlanCourses.Select(c => c.Id)).ToArray();
            if (nonExistingStudyPlanCourses.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following study plan courses don't exist.",
                        Data = new Dictionary<string, object>
                            {["NonExistingStudyPlanCourses"] = nonExistingStudyPlanCourses}
                    });
            }

            _dbContext.StudyPlansCourses.RemoveRange(existingStudyPlanCourses);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}