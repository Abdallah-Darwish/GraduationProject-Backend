using AutoMapper;
using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.StudyPlanCourseCategories;
using GradProjectServer.DTO.StudyPlans;
using GradProjectServer.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Mapping.StudyPlans
{
    public class StudyPlanToStudyPlanDtoConverter : ITypeConverter<StudyPlan, StudyPlanDto>
    {
        public StudyPlanDto Convert(StudyPlan src, StudyPlanDto dst, ResolutionContext ctx)
        {
            dst.Categories = ctx.Mapper.Map<StudyPlanCourseCategoryDto[]>(src.Categories);
            dst.CoursesData = ctx.Mapper.Map<CourseDto[]>(src.Courses.Select(c => c.Course).Distinct());
            dst.Courses = ctx.Mapper.Map<StudyPlanCourseDto[]>(src.Courses);
            dst.Id = src.Year;
            dst.Year = src.Year;
            dst.MajorId = src.MajorId;
            return dst;
        }
    }
}
