using GradProjectServer.DTO.StudyPlans;

namespace GradProjectServer.DTO.Majors
{
    public class MajorDto : MajorMetadataDto
    {
        public StudyPlanMetadataDto[] StudyPlans { get; set; }
    }
}
