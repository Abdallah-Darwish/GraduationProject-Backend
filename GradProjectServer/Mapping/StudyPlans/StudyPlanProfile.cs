using AutoMapper;
using GradProjectServer.DTO.StudyPlans;
using GradProjectServer.Services.Infrastructure;
using System.Linq;
namespace GradProjectServer.Mapping.StudyPlans
{
    public class StudyPlanProfile : Profile
    {
        public StudyPlanProfile()
        {
            CreateMap<StudyPlanCourse, StudyPlanCourseDto>()
                .ForMember(d => d.PrerequisiteCourses, op => op.MapFrom(e => e.Prerequisites.Select(p => p.PrerequisiteId)));
            CreateMap<StudyPlan, StudyPlanMetadataDto>();
            CreateMap<StudyPlan, StudyPlanDto>()
                .ConvertUsing<StudyPlanToStudyPlanDtoConverter>();
        }
    }
}
