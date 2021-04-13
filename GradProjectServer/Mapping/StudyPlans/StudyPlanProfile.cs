using AutoMapper;
using GradProjectServer.DTO.StudyPlans;
using GradProjectServer.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
