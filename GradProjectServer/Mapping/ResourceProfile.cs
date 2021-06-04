using AutoMapper;
using GradProjectServer.DTO.Resources;
using GradProjectServer.Services.Resources;

namespace GradProjectServer.Mapping
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile()
        {
            CreateMap<Resource, ResourceDto>();
        }
    }
}