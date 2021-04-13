using AutoMapper;
using GradProjectServer.DTO.ExamSubQuestions;
using GradProjectServer.Services.Exams.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
