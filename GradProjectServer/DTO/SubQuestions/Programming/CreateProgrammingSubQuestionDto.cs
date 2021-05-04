using GradProjectServer.DTO.Programs;

namespace GradProjectServer.DTO.SubQuestions
{
    public class CreateProgrammingSubQuestionDto : CreateSubQuestionDto
    {
        public string CheckerBase64 { get; set; }
        public CreateFileDto KeyAnswer { get; set; }
    }
}