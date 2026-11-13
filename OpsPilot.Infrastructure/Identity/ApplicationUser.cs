using Microsoft.AspNetCore.Identity;

namespace OpsPilot.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string? FullName { get; set; }
    public bool IsActive { get; set; } = true;
}
