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
using GradProjectServer.DTO.StudyPlans;
using GradProjectServer.Services.Infrastructure;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudyPlanCourseCategoryController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public StudyPlanCourseCategoryController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new study plan course category.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <reponse code="201">The newly created study plan course category.</reponse>
        [AdminFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(StudyPlanCourseCategoryDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<StudyPlanCourseCategoryDto>> Create([FromBody] CreateStudyPlanCourseCategoryDto info)
        {
            StudyPlanCourseCategory newStudyPlanCourseCategory = new()
            {
                CategoryId = info.CategoryId,
                StudyPlanId = info.StudyPlanId,
                AllowedCreditHours = info.AllowedCreditHours
            };
            await _dbContext.StudyPlansCoursesCategories.AddAsync(newStudyPlanCourseCategory).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return StatusCode(StatusCodes.Status201Created,
                _mapper.Map<StudyPlanCourseCategoryDto>(newStudyPlanCourseCategory));
        }

        /// <summary>
        /// Updates a study plan course category.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <param name="update">The update to apply, null fields mean no update to this property.</param>
        [AdminFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateStudyPlanCourseCategoryDto update)
        {
            var studyPlanCourseCategory = await _dbContext.StudyPlansCoursesCategories.FindAsync(update.Id).ConfigureAwait(false);
            if (update.CategoryId != null)
            {
                studyPlanCourseCategory.CategoryId = update.CategoryId.Value;
            }
            if (update.StudyPlanId != null)
            {
                studyPlanCourseCategory.StudyPlanId = update.StudyPlanId.Value;
            } if (update.AllowedCreditHours != null)
            {
                studyPlanCourseCategory.AllowedCreditHours = update.AllowedCreditHours.Value;
            }
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Deletes the specified study plan course categories.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <param name="studyPlanCourseCategoriesIds">Ids of the study plan course categories to delete.</param>
        /// <response code="404">Ids of the non existing study plan course categories.</response>
        [AdminFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] int[] studyPlanCourseCategoriesIds)
        {
            var existingStudyPlanCoursesCategories = _dbContext.StudyPlansCoursesCategories.Where(c => studyPlanCourseCategoriesIds.Contains(c.Id));
            var nonExistingStudyPlanCourseCategories =
                studyPlanCourseCategoriesIds.Except(existingStudyPlanCoursesCategories.Select(c => c.Id)).ToArray();
            if (nonExistingStudyPlanCourseCategories.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following study plan course categories don't exist.",
                        Data = new Dictionary<string, object>
                            {["NonExistingStudyPlanCourseCategories"] = nonExistingStudyPlanCourseCategories}
                    });
            }

            _dbContext.StudyPlansCoursesCategories.RemoveRange(existingStudyPlanCoursesCategories);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}