using System.ComponentModel.DataAnnotations;
using PureGym.Domain.Interfaces;

namespace PureGym.Infrastructure.Settings;

public class JwtSettings : ISettings
{
    public static string SectionName => "JwtSettings";

    [Required]
    public string Secret { get; set; } = null!;

    [Required]
    public string Issuer { get; set; } = null!;

    [Required]
    public string Audience { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; set; } = 60;
}
