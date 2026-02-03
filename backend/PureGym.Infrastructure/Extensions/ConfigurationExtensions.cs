using Microsoft.Extensions.Configuration;
using PureGym.SharedKernel.Interfaces;

namespace PureGym.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    public static T GetConfiguration<T>(this IConfiguration configuration) where T : ISettings, new()
    {
        return configuration.GetSection(T.SectionName).Get<T>()
            ?? throw new InvalidOperationException($"{T.SectionName} settings are not configured");
    }
}
