using AutoMapper;
using GradProjectServer.DTO.Majors;
using GradProjectServer.Services.Infrastructure;

namespace GradProjectServer.Mapping
{
    public class MajorProfile : Profile
    {
        public MajorProfile()
        {
            CreateMap<Major, MajorMetadataDto>();
            CreateMap<Major, MajorDto>();
        }
    }
}
