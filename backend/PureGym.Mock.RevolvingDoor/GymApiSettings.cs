using PureGym.SharedKernel.Interfaces;

namespace PureGym.Mock.RevolvingDoor;

public class GymApiSettings : ISettings
{
    public static string SectionName => "GymApi";
    public string BaseUrl { get; set; } = string.Empty;
    public string VerifyEndpoint { get; set; } = "api/access/verify";
    public int TimeoutSeconds { get; set; } = 5;
}
