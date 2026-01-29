using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PureGym.Application.Interfaces;
using PureGym.Application.Interfaces.Services;
using PureGym.Infrastructure;
using PureGym.Infrastructure.Persistence;
using PureGym.Infrastructure.Services;
using PureGym.Infrastructure.Settings;
using System.Text;


namespace FitNetClean.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGeneratedSettings(configuration);

        var dbConnection = configuration.GetSection(DatabaseSettings.SectionName)?.Get<DatabaseSettings>()?.ConnectionString ?? throw new InvalidOperationException($"{DatabaseSettings.SectionName} configuration value cannot be null");

        var rabbitMqSettings = configuration.GetSection(RabbitMQSettings.SectionName).Get<RabbitMQSettings>()
            ?? throw new InvalidOperationException("RabbitMQ settings are not configured");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(dbConnection));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.UsePostgres();
                o.DisableInboxCleanupService();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(rabbitMqSettings.Host, "/", h =>
                {
                    h.Username(rabbitMqSettings.UserName);
                    h.Password(rabbitMqSettings.Password);
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            options.User.RequireUniqueEmail = true;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured");

        var googleSettings = configuration.GetSection(GoogleSettings.SectionName).Get<GoogleSettings>()
        ?? throw new InvalidOperationException("Google settings are not configured");

        var facebookSettings = configuration.GetSection(FacebookSettings.SectionName).Get<FacebookSettings>()
        ?? throw new InvalidOperationException("Facebook settings are not configured");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero
            };
        })
        .AddGoogle(options =>
        {
            options.ClientId = googleSettings.ClientId;
            options.ClientSecret = googleSettings.ClientSecret;
        })
        .AddFacebook(options =>
        {
            options.AppId = facebookSettings.AppId;
            options.AppSecret = facebookSettings.AppSecret;
        });

        services.AddAuthorization();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<ICacheService, InMemoryCacheService>();

        return services;
    }
}
