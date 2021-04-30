using AutoMapper;
using GradProjectServer.DTO.StudyPlanCourseCategories;
using GradProjectServer.Services.Infrastructure;

namespace GradProjectServer.Mapping
{
    public class StudyPlanCourseCategoryProfile : Profile
    {
        public StudyPlanCourseCategoryProfile()
        {
            CreateMap<StudyPlanCourseCategory, StudyPlanCourseCategoryDto>();
        }
    }
}