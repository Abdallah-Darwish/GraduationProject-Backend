using AutoMapper;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.DTO.SubQuestions.Programming;
using GradProjectServer.DTO.Tags;
using GradProjectServer.Services.Exams.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Mapping.SubQuestions
{
    public class SubQuestionProfile : Profile
    {
        public SubQuestionProfile()
        {
            CreateMap<SubQuestion, SubQuestionMetadataDto>();

            CreateMap<BlankSubQuestion, OwnedBlankSubQuestionDto>();

            CreateMap<MCQSubQuestionChoice, OwnedMCQSubQuestionChoiceDto>();
            CreateMap<MCQSubQuestion, MCQSubQuestionMetadataDto>();
            CreateMap<MCQSubQuestion, MCQSubQuestionDto>();

            CreateMap<ProgrammingSubQuestion, OwnedProgrammingSubQuestionDto>();

        }
    }
}
