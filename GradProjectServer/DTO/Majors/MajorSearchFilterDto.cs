namespace GradProjectServer.DTO.Majors
{
    public class MajorSearchFilterDto
    {
        public string? NameMask { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
