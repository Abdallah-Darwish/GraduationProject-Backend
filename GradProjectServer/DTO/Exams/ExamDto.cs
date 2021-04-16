using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.ExamQuestions;
using GradProjectServer.DTO.Questions;
using GradProjectServer.DTO.SubQuestions;

namespace GradProjectServer.DTO.Exams
{
    public class ExamDto : ExamMetadataDto
    {
        //todo: order and order sub questions
        public ExamQuestionDto[] Questions { get; set; }
        //maybe we should add tags here !
    }
}