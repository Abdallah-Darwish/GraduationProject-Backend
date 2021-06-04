namespace DockerBroker.Services
{
    public class BuildJob : Job
    {
        public string RelativeArchivePath { get; set; }
        public string RelativeSavePath { get; set; }
    }
}