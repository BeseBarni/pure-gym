using PureGym.SharedKernel.Interfaces;

namespace PureGym.SharedKernel.Settings;

public class FacebookSettings : ISettings
{
    public static string SectionName => "Facebook";
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
}
