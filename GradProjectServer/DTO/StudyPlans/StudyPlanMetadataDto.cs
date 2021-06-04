using GradProjectServer.DTO.Majors;

namespace GradProjectServer.DTO.StudyPlans
{
    public class StudyPlanMetadataDto
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public MajorMetadataDto Major { get; set; }
    }
}