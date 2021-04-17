using AutoMapper;
using GradProjectServer.DTO.Programs;
using GradProjectServer.Services.Infrastructure;

namespace GradProjectServer.Mapping
{
    public class ProgramProfile : Profile
    {
        public ProgramProfile()
        {
            CreateMap<Dependency, DependencyDto>();

            CreateMap<ProgramDependency, DependencyDto>()
                .ForMember(d => d.Id, op => op.MapFrom(e => e.DependecyId))
                .ForMember(d => d.Name, op => op.MapFrom(e => e.Dependency.Name));

            CreateMap<Program, ProgramDto>();
        }
    }
}
