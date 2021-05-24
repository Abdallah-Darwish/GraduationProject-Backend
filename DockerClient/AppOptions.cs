namespace DockerClientASP
{
    public class AppOptions
    {
        public const string SectionName = "App";
        /// <summary>
        /// Created so we could save files on another server and use multiple containers at the same time with a web server like nginx.
        /// </summary>
        public string DataSaveDirectory { get; set; }
    }
}