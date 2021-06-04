using GradProjectServer.DTO.ExamQuestions;

namespace GradProjectServer.DTO.Exams
{
    public class ExamDto : ExamMetadataDto
    {
        public ExamQuestionDto[] Questions { get; set; }
        //maybe we should add tags here !
    }
}