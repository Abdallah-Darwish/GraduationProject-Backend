namespace GradProjectServer.DTO.Programs
{
    public class CreateProgramDto
    {
        public string ArchiveBase64 { get; set; }
        public int[] Dependencies { get; set; }
    }
}
