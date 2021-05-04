using GradProjectServer.DTO.StudyPlans;

namespace GradProjectServer.DTO.Users
{
    public class UserDto : UserMetadataDto
    {
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public StudyPlanMetadataDto StudyPlan { get; set; }
    }
}