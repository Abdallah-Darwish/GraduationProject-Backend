using GradProjectServer.DTO.Programs;

namespace GradProjectServer.DTO.SubQuestions
{
    public class CreateProgrammingSubQuestionDto : CreateSubQuestionDto
    {
        public CreateProgramDto Checker { get; set; }
    }
}
