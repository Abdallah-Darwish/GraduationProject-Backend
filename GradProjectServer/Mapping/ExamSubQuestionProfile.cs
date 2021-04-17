using AutoMapper;
using GradProjectServer.DTO.ExamSubQuestions;
using GradProjectServer.Services.Exams.Entities;

namespace GradProjectServer.Mapping
{
    public class ExamSubQuestionProfile : Profile
    {
        public ExamSubQuestionProfile()
        {
            CreateMap<ExamSubQuestion, ExamSubQuestionDto>();
        }
    }
}
