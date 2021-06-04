using AutoMapper;
using GradProjectServer.DTO.Questions;
using GradProjectServer.Services.Exams.Entities;

namespace GradProjectServer.Mapping
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<Question, QuestionMetadataDto>();
            CreateMap<Question, QuestionDto>();
        }
    }
}