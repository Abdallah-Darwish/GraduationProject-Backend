namespace GradProjectServer.DTO.Tags
{
    public class TagSearchFilterDto
    {
        public string? NameMask { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}