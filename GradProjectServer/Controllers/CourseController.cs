using AutoMapper;
using GradProjectServer.DTO;
using GradProjectServer.DTO.Courses;
using GradProjectServer.Services.EntityFramework;
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
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public CourseController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        //todo: add order by to all get all methods
        [HttpPost("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<int>> GetAll([FromBody] GetAllDto info)
        {
            return Ok(_dbContext.Courses.OrderBy(c => c.Name).Skip(info.Offset).Take(info.Count).Select(c => c.Id));
        }

        [HttpPost("Get")]
        [ProducesResponseType(typeof(IEnumerable<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<CourseDto>> Get([FromBody] int[] coursesIds)
        {
            var existingCourses = _dbContext.Courses.Where(c => coursesIds.Contains(c.Id));
            var nonExistingCourses = coursesIds.Except(existingCourses.Select(c => c.Id)).ToArray();
            if (nonExistingCourses.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following courses don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingCourses"] = nonExistingCourses }
                        });
            }
            return Ok(_mapper.ProjectTo<CourseDto>(existingCourses));
        }
        [AdminFilter]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto info)
        {
            var newCourse = new Course
            {
                Name = info.Name,
                CreditHours = info.CreditHours
            };
            await _dbContext.Courses.AddAsync(newCourse).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new { coursesIds = new int[] { newCourse.Id } }, newCourse);
        }
        [AdminFilter]
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] int[] coursesIds)
        {
            var existingCourses = _dbContext.Courses.Where(c => coursesIds.Contains(c.Id));
            var nonExistingCourses = coursesIds.Except(existingCourses.Select(c => c.Id)).ToArray();
            if (nonExistingCourses.Length > 0)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new ErrorDTO
                        {
                            Description = "The following courses don't exist.",
                            Data = new Dictionary<string, object> { ["NonExistingCourses"] = nonExistingCourses }
                        });
            }
            _dbContext.Courses.RemoveRange(existingCourses);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        [AdminFilter]
        [HttpPatch("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateCourseDto update)
        {
            var course = await _dbContext.Courses.FindAsync(update.Id).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(update.Name))
            {
                course.Name = update.Name;
            }
            if (update.CreditHours.HasValue)
            {
                course.CreditHours = update.CreditHours.Value;
            }
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("Search")]
        [ProducesResponseType(typeof(IEnumerable<CourseDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CourseDto>> Search([FromBody] CourseSearchFilterDto filter)
        {
            var courses = _dbContext.Courses.AsQueryable();

            if (filter.NameMask != null)
            {
                courses = courses.Where(c => EF.Functions.Like(c.Name, filter.NameMask));
            }
            if (filter.MinCreditHours != null)
            {
                courses = courses.Where(c => c.CreditHours >= filter.MinCreditHours);
            }
            if (filter.MaxCreditHours != null)
            {
                courses = courses.Where(e => e.CreditHours <= filter.MaxCreditHours);
            }
            if (filter.HasExams.HasValue)
            {
                courses = courses.Where(c => c.Exams.Any() == filter.HasExams);
            }
            var result = courses.OrderBy(e => e.Name).Skip(filter.Offset).Take(filter.Count);
            return Ok(_mapper.ProjectTo<CourseDto>(result));
        }
    }
}
