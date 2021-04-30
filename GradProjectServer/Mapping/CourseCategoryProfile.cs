using AutoMapper;
using GradProjectServer.DTO.CourseCategories;
using GradProjectServer.Services.Infrastructure;

namespace GradProjectServer.Mapping
{
    public class CourseCategoryProfile : Profile
    {
        public CourseCategoryProfile()
        {
            CreateMap<CourseCategory, CourseCategoryDto>();
        }
    }
}