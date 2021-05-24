namespace DockerClient
{
    public class AppOptions
    {
        public const string SectionName = "App";

        /// <summary>
        /// Path to data shared between backend and docker clients (a shared storage)
        /// </summary>
        public string CentralDataDirectory { get; set; }

        /// <summary>
        /// Where this client can save its docker volumes directories for builds
        /// </summary>
        public string DataSaveDirectory { get; set; }

        public string ServerAddress { get; set; }
        public int ServerPullPort { get; set; }
        public int ServerPushPort { get; set; }
        public string ClientPullAddress => $">tcp://{ServerAddress}:{ServerPushPort}";
        public string ClientPushAddress => $">tcp://{ServerAddress}:{ServerPullPort}";
    }
}