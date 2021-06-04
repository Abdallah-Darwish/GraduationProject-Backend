using AutoMapper;
using GradProjectServer.DTO.ExamAttempts;
using GradProjectServer.Services.Exams.Entities.ExamAttempts;

namespace GradProjectServer.Mapping
{
    public class ExamAttemptProfile : Profile
    {
        public ExamAttemptProfile()
        {
            CreateMap<ExamAttempt, ExamAttemptDto>();
        }
    }
}