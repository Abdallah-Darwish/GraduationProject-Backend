using Npgsql;

namespace GradProjectServer
{
    public class AppOptions
    {
        public const string SectionName = "App";
        public string DbName { get; set; }
        public string PostgresUserId { get; set; }
        public string PostgresPassword { get; set; }
        public string DataSaveDirectory { get; set; }
        public string PostgresServerAddress { get; set; }
        public int PostgresMaxAutoPrepare { get; set; }

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

        public string BuildPostgresConnectionString()=> BuildConnectionString("postgres");
    }
}