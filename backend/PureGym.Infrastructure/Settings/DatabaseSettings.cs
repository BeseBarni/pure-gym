using PureGym.Domain.Interfaces;

namespace PureGym.Infrastructure.Settings;

public sealed class DatabaseSettings : ISettings
{
    public string ConnectionString { get; set; } = string.Empty;
}
