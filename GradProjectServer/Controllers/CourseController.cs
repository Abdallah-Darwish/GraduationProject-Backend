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
        [HttpGet]
        public ActionResult<CourseDto[]> Get([FromBody] int[] coursesIds)
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
        [HttpPost]
        [AdminFilter]
        public async Task<ActionResult<CourseDto>> Create(CreateCourseDto info)
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
        [HttpDelete]
        [AdminFilter]
        public async Task<IActionResult> Delete(int[] coursesIds)
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
        [HttpPatch]
        [AdminFilter]
        public async Task<IActionResult> Update(UpdateCourseDto update)
        {
            var course = await _dbContext.Courses.FindAsync(update.Id).ConfigureAwait(false);
            if(!string.IsNullOrWhiteSpace(update.Name))
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

        [HttpGet]
        public IActionResult Search([FromBody] CourseSearchFilterDto filter)
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
