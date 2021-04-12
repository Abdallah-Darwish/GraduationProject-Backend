using AutoMapper;
using GradProjectServer.DTO.Questions;
using GradProjectServer.Services.Exams.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Mapping
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<Question, QuestionMetadataDto>();
            CreateMap<Question, QuestionDto>();
            CreateMap<Question, QuestionDto>();
        }
    }
}
