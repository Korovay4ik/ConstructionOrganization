using Microsoft.EntityFrameworkCore;

namespace constructionOrgManagement.Models
{
    public class DatabaseConnectionSettings
    {
        public required string Host { get; set; }
        public required string Database { get; set; }
        public required string UserLogin { get; set; }
        public required string UserPassword { get; set; }
    }
    public partial class ConstructionOrganizationContext
    {
        private readonly DatabaseConnectionSettings? _connectionSettings;

        public ConstructionOrganizationContext(DatabaseConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionSettings == null) return;

            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = $"host={_connectionSettings.Host};" +
                                     $"database={_connectionSettings.Database};" +
                                     $"username={_connectionSettings.UserLogin};" +
                                     $"password={_connectionSettings.UserPassword}";

                optionsBuilder.UseMySql(connectionString,ServerVersion.Parse("8.2.0-mysql"));
            }
        }
    }
}
