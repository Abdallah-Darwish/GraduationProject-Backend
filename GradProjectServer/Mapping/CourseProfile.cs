using AutoMapper;
using GradProjectServer.DTO.Courses;
using GradProjectServer.Services.Infrastructure;

namespace GradProjectServer.Mapping
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDto>();
        }
    }
}