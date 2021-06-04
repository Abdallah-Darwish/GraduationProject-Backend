namespace DockerBroker
{
    public class AppOptions
    {
        public const string SectionName = "App";
        public int ServerPullPort { get; set; }
        public int ServerPushPort { get; set; }
        public string ServerPullAddress => $"@tcp://*:{ServerPullPort}";
        public string ServerPushAddress => $"@tcp://*:{ServerPushPort}";
    }
}