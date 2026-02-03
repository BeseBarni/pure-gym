using PureGym.SharedKernel.Interfaces;

namespace PureGym.SharedKernel.Settings;

public sealed class GymEntrySettings : ISettings
{
    public static string SectionName => "GymEntry";
    public int EntryKeyCacheTime { get; set; } = 30;
}
