using GradProjectServer.Common;

namespace GradProjectServer.DTO.SubQuestions
{
    public abstract class CreateSubQuestionDto
    {
        public string Content { get; set; }
        public SubQuestionType Type { get; set; }
        public int[]? Tags { get; set; }
        public int QuestionId { get; set; }
    }
}