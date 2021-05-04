namespace GradProjectServer.DTO.Programs
{
    public class CreateFileDto
    {
        public string ContentBase64 { get; set; }

        public string FileExtension { get; set; }
        //todo: add programing language here as enum with unknown field
    }
}