using Microsoft.AspNetCore.Identity;
using PureGym.Infrastructure.Extensions;
using PureGym.Infrastructure.Persistence;

namespace PureGym.WebAPI.Extensions;

public static class AppExtensions
{
    public static async Task UseApplyMigrations(this WebApplication app)
    {
        if (!(app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("APPLY_MIGRATIONS") == "true")) return;

        Console.WriteLine("Waiting for database to be ready...");
        var dbReady = await app.Services.WaitForDatabaseAsync<ApplicationDbContext>();

        if (dbReady)
        {
            await app.Services.ApplyMigrationsAsync<ApplicationDbContext>();
        }
        else
        {
            Console.WriteLine("WARNING: Could not connect to database. Application may not function correctly.");
        }
    }

    public static async Task UseDatabaseSeed(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var ct = new CancellationToken();
        await DatabaseSeeder.SeedTestDataAsync(context, ct);
    }
}
