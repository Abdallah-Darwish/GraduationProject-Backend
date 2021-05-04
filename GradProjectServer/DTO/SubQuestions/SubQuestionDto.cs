using GradProjectServer.DTO.Tags;

namespace GradProjectServer.DTO.SubQuestions
{
    public class SubQuestionDto : SubQuestionMetadataDto
    {
        public string Content { get; set; }
        public TagDto[] Tags { get; set; }
    }
}