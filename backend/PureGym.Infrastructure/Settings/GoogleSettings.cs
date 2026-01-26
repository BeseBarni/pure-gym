using PureGym.SharedKernel.Interfaces;

namespace PureGym.Infrastructure.Settings;

public class GoogleSettings : ISettings
{
    public static string SectionName => "Google";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
