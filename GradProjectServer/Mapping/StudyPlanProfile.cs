using AutoMapper;
using GradProjectServer.DTO.StudyPlans;
using GradProjectServer.Services.Infrastructure;
using System.Linq;

namespace GradProjectServer.Mapping
{
    public class StudyPlanProfile : Profile
    {
        public StudyPlanProfile()
        {
            CreateMap<StudyPlan, StudyPlanMetadataDto>();
            CreateMap<StudyPlan, StudyPlanDto>();
        }
    }
}