using GradProjectServer.DTO.Programs;
namespace GradProjectServer.DTO.SubQuestions
{
    public class UpdateProgrammingSubQuestionDto : UpdateSubQuestionDto
    {
        public CreateProgramDto? Checker { get; set; }
    }
}
