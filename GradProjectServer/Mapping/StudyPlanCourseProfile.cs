using System.Linq;
using AutoMapper;
using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.StudyPlanCourses;
using GradProjectServer.Services.Infrastructure;

namespace GradProjectServer.Mapping
{
    public class StudyPlanCourseConverter : ITypeConverter<StudyPlanCourse, StudyPlanCourseDto>
    {
        public StudyPlanCourseDto Convert(StudyPlanCourse src, StudyPlanCourseDto dst, ResolutionContext ctx)
        {
            dst.Course = ctx.Mapper.Map<CourseDto>(src.Course);
            dst.StudyPlanCourseCategoryId = src.CategoryId;
            dst.Prerequisites = src.Prerequisites
                .Select(p => new StudyPlanCoursePrerequisiteDto
                {
                    StudyPlanCourseId = p.PrerequisiteId,
                    Course = ctx.Mapper.Map<CourseDto>(p.Prerequisite.Course)
                })
                .ToArray();
            return dst;
        }
    }

    public class StudyPlanCourseProfile : Profile
    {
        public StudyPlanCourseProfile()
        {
            CreateMap<StudyPlanCoursePrerequisite, StudyPlanCoursePrerequisiteDto>()
                .ForMember(d => d.StudyPlanCourseId, op => op.MapFrom(e => e.PrerequisiteId))
                .ForMember(d => d.Course, op => op.MapFrom(e => e.Prerequisite.Course));

            CreateMap<StudyPlanCourse, StudyPlanCourseDto>()
                .ForMember(d => d.StudyPlanCourseCategoryId, op => op.MapFrom(e => e.CategoryId));
        }
    }
}