using AutoMapper;
using GradProjectServer.DTO.Tags;
using GradProjectServer.Services.Exams.Entities;

namespace GradProjectServer.Mapping
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<Tag, TagDto>();

            CreateMap<SubQuestionTag, TagDto>()
                .ForMember(d => d.Id, c => c.MapFrom(e => e.TagId))
                .ForMember(d => d.Name, c => c.MapFrom(e => e.Tag.Name));
        }
    }
}
