using Microsoft.AspNetCore.Identity;

namespace PureGym.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastLoginUtc { get; set; }
    
    public Guid? MemberId { get; set; }
    public Member? Member { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}
