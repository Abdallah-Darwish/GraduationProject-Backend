using AutoMapper;
using GradProjectServer.DTO.Exams;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Mapping.Exams
{
    public class ExamProfile : Profile
    {
        public ExamProfile()
        {
            CreateMap<Exam, ExamMetadataDto>();
            CreateMap<Exam, ExamDto>()
                .ConvertUsing<ExamToExamDtoConverter>();

        }
    }
}
