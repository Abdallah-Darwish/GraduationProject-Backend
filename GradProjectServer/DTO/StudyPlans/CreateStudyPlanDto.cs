namespace GradProjectServer.DTO.StudyPlans
{
    //todo: validate no plan with same major and id
    public class CreateStudyPlanDto
    {
        public int MajorId { get; set; }
        public int Year { get; set; }
    }
}