using PureGym.SharedKernel.Interfaces;

namespace PureGym.SharedKernel.Settings;

public class DatabaseSettings : ISettings
{
    public static string SectionName => "Database";
    public string ConnectionString { get; set; } = string.Empty;
}
