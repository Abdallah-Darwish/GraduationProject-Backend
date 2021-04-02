using GradProjectServer.Common;

namespace GradProjectServer.DTO.SubQuestions
{
    public class SubQuestionMetadataDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public SubQuestionType Type { get; set; }
    }
}
