namespace GradProjectServer.DTO.ExamQuestions
{
    //todo: validate exam is not approved or is admin
    //todo: validate question is approved
    //todo: validate exam is owned by user or is admin
    //todo: validate question course is same as exam
    public class CreateExamQuestionDto
    {
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int Order { get; set; }
    }
}