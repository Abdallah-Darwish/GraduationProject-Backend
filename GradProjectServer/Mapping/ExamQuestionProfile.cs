using AutoMapper;
using GradProjectServer.DTO.ExamQuestions;
using GradProjectServer.Services.Exams.Entities;

namespace GradProjectServer.Mapping
{
    public class ExamQuestionProfile : Profile
    {
        public ExamQuestionProfile()
        {
            CreateMap<ExamQuestion, ExamQuestionDto>()
                .ForMember(d => d.Content, op => op.MapFrom(e => e.Question.Content))
                .ForMember(d => d.Title, op => op.MapFrom(e => e.Question.Title));
        }
    }
}