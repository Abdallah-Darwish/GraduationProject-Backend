using AutoMapper;
using GradProjectServer.DTO.Exams;
using GradProjectServer.Services.Exams.Entities;

namespace GradProjectServer.Mapping.Exams
{
    public class ExamProfile : Profile
    {
        public ExamProfile()
        {
            CreateMap<Exam, ExamMetadataDto>()
                .ConvertUsing<ExamToExamMetadataConverter>();
            CreateMap<Exam, ExamDto>()
                .ConvertUsing<ExamToExamDtoConverter>();

        }
    }
}
