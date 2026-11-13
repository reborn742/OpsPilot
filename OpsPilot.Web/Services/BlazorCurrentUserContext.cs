using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using OpsPilot.Application.Common;
using OpsPilot.Application.Interfaces;
using OpsPilot.Infrastructure.Identity;

namespace OpsPilot.Web.Services;

public sealed class BlazorCurrentUserContext : ICurrentUserContext
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly UserManager<ApplicationUser> _userManager;

    public BlazorCurrentUserContext(
        AuthenticationStateProvider authenticationStateProvider,
        UserManager<ApplicationUser> userManager)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _userManager = userManager;
    }

    public async Task<AuditActor> GetCurrentActorAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var principal = state.User;

        if (principal.Identity?.IsAuthenticated != true)
            return new AuditActor(null, "Anonymous");

        var userIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            throw new InvalidOperationException("Authenticated user ID is missing or invalid.");

        var user = await _userManager.FindByIdAsync(userId.ToString());

        return new AuditActor(
            userId,
            user?.FullName ?? principal.Identity.Name ?? "Unknown user");
    }
}
