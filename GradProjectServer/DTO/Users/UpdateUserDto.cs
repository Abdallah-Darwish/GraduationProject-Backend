namespace GradProjectServer.DTO.Users
{
    //todo: validate is logged in user or admin is updating himself or non admins
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public bool? IsAdmin { get; set; }
        public string? Password { get; set; }
        public string? ProfilePictureJpgBase64 { get; set; }
        public int? StudyPlanId { get; set; }
        public string? Name { get; set; }
    }
}