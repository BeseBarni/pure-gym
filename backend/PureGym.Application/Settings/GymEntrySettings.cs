using PureGym.SharedKernel.Interfaces;

namespace PureGym.Application.Settings;

public sealed class GymEntrySettings : ISettings
{
    public static string SectionName => "GymEntry";
    public int EntryKeyCacheTime { get; set; } = 30;
}
