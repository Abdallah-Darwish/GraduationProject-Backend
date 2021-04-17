using AutoMapper;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.Exams.Entities;

namespace GradProjectServer.Mapping.SubQuestions
{
    public class SubQuestionProfile : Profile
    {
        public SubQuestionProfile()
        {
            CreateMap<SubQuestion, SubQuestionMetadataDto>();
            CreateMap<SubQuestion, SubQuestionDto>()
                .IncludeAllDerived();

            CreateMap<BlankSubQuestion, OwnedBlankSubQuestionDto>();

            CreateMap<MCQSubQuestionChoice, OwnedMCQSubQuestionChoiceDto>();
            CreateMap<MCQSubQuestionChoice, MCQSubQuestionChoiceDto>();
            CreateMap<MCQSubQuestion, MCQSubQuestionMetadataDto>();
            CreateMap<MCQSubQuestion, MCQSubQuestionDto>();
            CreateMap<MCQSubQuestion, OwnedMCQSubQuestionDto>();

            CreateMap<ProgrammingSubQuestion, OwnedProgrammingSubQuestionDto>();
        }
    }
}
