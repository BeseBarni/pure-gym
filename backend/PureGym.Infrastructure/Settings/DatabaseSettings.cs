using PureGym.Domain.Interfaces;

namespace PureGym.Infrastructure.Settings;

public class DatabaseSettings : ISettings
{
    public static string SectionName => "Database";
    public string ConnectionString { get; set; } = string.Empty;
}
