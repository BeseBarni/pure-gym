using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PureGym.Infrastructure.Persistence;

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

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use the same connection string name as configured in Aspire
        var connectionString = configuration.GetConnectionString("puregymdb");

        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback for design-time when Aspire connection string is not available
            // This uses a local PostgreSQL instance with a design-time specific database name
            connectionString = "Host=localhost;Database=puregymdb_design;Username=postgres;Password=postgres";
            Console.WriteLine($"Warning: Using fallback connection string for design-time. " +
                            $"Current directory: {currentDir}, Config base path: {basePath}");
        }

        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        });

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
