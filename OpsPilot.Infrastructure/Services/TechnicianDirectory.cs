using Microsoft.AspNetCore.Identity;
using OpsPilot.Application.DTOs.Technicians;
using OpsPilot.Application.Interfaces;
using OpsPilot.Infrastructure.Identity;

namespace OpsPilot.Infrastructure.Services;

public sealed class TechnicianDirectory : ITechnicianDirectory
{
    private readonly UserManager<ApplicationUser> _userManager;

    public TechnicianDirectory(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<TechnicianDto>> GetActiveTechniciansAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var users = await _userManager.GetUsersInRoleAsync(RoleNames.Technician);

        return users
            .Where(user => user.IsActive)
            .OrderBy(user => user.FullName ?? user.UserName)
            .Select(user => new TechnicianDto(
                user.Id,
                user.FullName ?? user.UserName ?? "Technician",
                user.Email ?? ""))
            .ToList();
    }

    public async Task<bool> IsActiveTechnicianAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (userId == Guid.Empty)
            return false;

        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user is not null
            && user.IsActive
            && await _userManager.IsInRoleAsync(user, RoleNames.Technician);
    }
}
