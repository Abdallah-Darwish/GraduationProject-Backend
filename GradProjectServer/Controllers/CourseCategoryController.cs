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
using GradProjectServer.DTO.StudyPlans;
using GradProjectServer.Services.Infrastructure;

namespace GradProjectServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseCategoryController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public CourseCategoryController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <reamarks>Result is ordered by name.</reamarks>
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            return Ok(_dbContext.CoursesCategories
                .OrderBy(s => s.Name)
                .Skip(info.Offset)
                .Take(info.Count)
                .Select(t => t.Id));
        }

        /// <param name="coursesCategoriesIds">Ids of the study plans to get.</param>
        /// <response code="404">Ids of the non existing study plans.</response>
        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<CourseCategory>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public IActionResult Get([FromBody] int[] coursesCategoriesIds)
        {
            var existingCoursesCategories =
                _dbContext.CoursesCategories.Where(c => coursesCategoriesIds.Contains(c.Id));
            var nonExistingCoursesCategories =
                coursesCategoriesIds.Except(existingCoursesCategories.Select(c => c.Id)).ToArray();
            if (nonExistingCoursesCategories.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following courses categories don't exist.",
                        Data = new Dictionary<string, object>
                            {["NonExistingCourseCategories"] = nonExistingCoursesCategories}
                    });
            }

            return Ok(_mapper.ProjectTo<CourseCategoryDto>(existingCoursesCategories));
        }

        /// <summary>
        /// Creates a new course category.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <reponse code="201">The newly created course category.</reponse>
        [AdminFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(CourseCategoryDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<StudyPlanMetadataDto>> Create([FromBody] CreateCourseCategoryDto info)
        {
            CourseCategory newCourseCategory = new()
            {
                Name = info.Name
            };
            await _dbContext.CoursesCategories.AddAsync(newCourseCategory).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new {tagsIds = new int[] {newCourseCategory.Id}},
                _mapper.Map<CourseCategoryDto>(newCourseCategory));
        }

        /// <summary>
        /// Updates a course category.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <param name="update">The update to apply, null fields mean no update to this property.</param>
        [AdminFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateCourseCategoryDto update)
        {
            var courseCategory = await _dbContext.CoursesCategories.FindAsync(update.Id).ConfigureAwait(false);
            if (update.Name != null)
            {
                courseCategory.Name = update.Name;
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Deletes the specified course category.
        /// </summary>
        /// <remarks>
        /// Admin only.
        /// </remarks>
        /// <param name="courseCategoriesIds">Ids of the course categories to delete.</param>
        /// <response code="404">Ids of the non existing course categories.</response>
        [AdminFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] int[] courseCategoriesIds)
        {
            var existingCoursesCategories = _dbContext.CoursesCategories.Where(c => courseCategoriesIds.Contains(c.Id));
            var nonExistingCourseCategories =
                courseCategoriesIds.Except(existingCoursesCategories.Select(c => c.Id)).ToArray();
            if (nonExistingCourseCategories.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorDTO
                    {
                        Description = "The following course categories don't exist.",
                        Data = new Dictionary<string, object>
                            {["NonExistingCourseCategories"] = nonExistingCourseCategories}
                    });
            }

            _dbContext.CoursesCategories.RemoveRange(existingCoursesCategories);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        /// <summary>
        /// Returns courses categories that satisfy the filters ordered by name.
        /// </summary>
        /// <param name="filter">The filters to apply, null property means it won't be applied.</param>
        [HttpPost("Search")]
        [ProducesResponseType(typeof(IEnumerable<CourseCategoryDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CourseCategoryDto>> Search([FromBody] CourseCategorySearchFilterDto filter)
        {
            var coursesCategories = _dbContext.CoursesCategories.AsQueryable();
            if (filter.NameMask != null)
            {
                coursesCategories = coursesCategories.Where(t => EF.Functions.Like(t.Name, filter.NameMask));
            }

            var result = coursesCategories
                .OrderBy(t => t.Name)
                .Skip(filter.Offset)
                .Take(filter.Count);
            return Ok(_mapper.ProjectTo<CourseCategoryDto>(coursesCategories));
        }
    }
}