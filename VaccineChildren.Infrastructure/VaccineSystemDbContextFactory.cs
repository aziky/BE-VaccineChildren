using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using VaccineChildren.Infrastructure.Configuration;

namespace VaccineChildren.Infrastructure
{
    public class VaccineSystemDbContextFactory : IDesignTimeDbContextFactory<VaccineSystemDbContext>
    {
        public VaccineSystemDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Lấy thư mục hiện tại
                .AddJsonFile("appsettings.Dev.json", optional: false, reloadOnChange: true) // Đọc file config
                .Build();
            
            // Đọc cấu hình database từ "DatabaseConnection"
            var databaseSettings = configuration.GetSection("DatabaseConnection").Get<DatabaseConnection>();
            
            if (databaseSettings == null)
            {
                throw new InvalidOperationException("Database connection settings are missing in appsettings.json");
            }

            var connectionString = databaseSettings.ToConnectionString();

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is missing in appsettings.json");
            }

            var optionsBuilder = new DbContextOptionsBuilder<VaccineSystemDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new VaccineSystemDbContext(optionsBuilder.Options);
        }
    }
}