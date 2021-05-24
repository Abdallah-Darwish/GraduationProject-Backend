using Npgsql;

namespace GradProjectServer
{
    public class AppOptions
    {
        public const string SectionName = "App";
        public string DbName { get; set; }
        public string PostgresUserId { get; set; }
        public string PostgresPassword { get; set; }
        public string PostgresServerAddress { get; set; }
        public int PostgresMaxAutoPrepare { get; set; }
        public string PostgresDefaultDbName { get; set; }

        /// <summary>
        /// Created so we could save files on another server and use multiple containers at the same time with a web server like nginx.
        /// </summary>
        public string DataSaveDirectory { get; set; }

        public string DockerBrokerAddress { get; set; }
        public int DockerBrokerPort{ get; set; }

        private string BuildConnectionString(string? dbName)
        {
            NpgsqlConnectionStringBuilder builder = new()
            {
                ApplicationName = "GradProjectServer",
                Database = dbName,
                Host = PostgresServerAddress,
                Username = PostgresUserId,
                Password = PostgresPassword,
                MaxAutoPrepare = PostgresMaxAutoPrepare,
                AutoPrepareMinUsages = 2,
            };
            return builder.ConnectionString;
        }

        public string BuildAppConnectionString() => BuildConnectionString(DbName);

        public string BuildPostgresConnectionString() => BuildConnectionString(PostgresDefaultDbName);
    }
}