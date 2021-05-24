namespace DockerBroker.Services
{
    public class CheckJob : Job
    {
        public string RelativeSubmissionDirectory { get; set; }
        public string RelativeCheckerDirectory { get; set; }
        public string RelativeResultDirectory { get; set; }
    }
}