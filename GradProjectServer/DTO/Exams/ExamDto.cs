using GradProjectServer.DTO.Questions;
using GradProjectServer.DTO.SubQuestions;

namespace GradProjectServer.DTO.Exams
{
    //todo: fill me
    public class ExamDto : ExamMetadataDto
    {
        public ExamQuestionDto[] Questions { get; set; }
        //maybe we should add tags here !
    }
}