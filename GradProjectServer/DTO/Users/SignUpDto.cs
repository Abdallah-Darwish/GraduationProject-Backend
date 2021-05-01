namespace GradProjectServer.DTO.Users
{
    //todo: validate email
    //todo: validate email is not used before
    //todo: validate password constraints
    //todo: validate ProfilePictureBase64 is a valid jpg
//todo: validate mail domain
    public class SignUpDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? ProfilePictureJpgBase64 { get; set; }
        public int StudyPlanId { get; set; }
    }
}
