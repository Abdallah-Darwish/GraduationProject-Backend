using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.DTO.Users;

namespace GradProjectServer.DTO.Questions
{
    public class QuestionDto : QuestionMetadataDto
    {
        public string Content { get; set; }
        public SubQuestionMetadataDto[] SubQuestions { get; set; }
        public UserMetadataDto Volunteer { get; set; }
    }
}
