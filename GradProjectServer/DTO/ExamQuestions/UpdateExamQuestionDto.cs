namespace GradProjectServer.DTO.ExamQuestions
{
    //todo: validate exam is not approved or is admin
    //todo: validate owner ship
    public class UpdateExamQuestionDto
    {
        public int Id { get; set; }
        public int? Order { get; set; }
    }
}
