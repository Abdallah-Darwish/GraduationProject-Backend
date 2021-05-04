using GradProjectServer.Common;
using GradProjectServer.DTO.Programs;

namespace GradProjectServer.DTO.SubQuestionAnswers.Programming
{
    public class CreateProgrammingSubQuestionAnswerDto : CreateSubQuestionAnswerDto
    {
        public CreateFileDto Answer { get; set; }
        public ProgrammingLanguage ProgrammingLanguage { get; set; }
    }
}