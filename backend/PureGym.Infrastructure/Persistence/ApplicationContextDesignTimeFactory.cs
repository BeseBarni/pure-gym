using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PureGym.Infrastructure.Persistence;
using PureGym.Infrastructure.Settings;

namespace FitNetClean.Infrastructure.Persistence;

public class ApplicationContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var currentDir = Directory.GetCurrentDirectory();
        var basePath = Path.Combine(currentDir, "..", "PureGym.WebAPI");

        if (!Directory.Exists(basePath))
        {
            basePath = Path.Combine(currentDir, "PureGym.WebAPI");
        }

        if (!Directory.Exists(basePath))
        {
            basePath = currentDir;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var databaseSettings = new DatabaseSettings();
        configuration.GetSection(DatabaseSettings.SectionName).Bind(databaseSettings);

        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();

        string? connectionString = databaseSettings.ConnectionString;

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"Could not find connection string in Database:ConnectionString. " +
                $"Current directory: {currentDir}, Config base path: {basePath}");
        }

        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
