using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace OpsPilot.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var roleName in RoleNames.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await roleManager.CreateAsync(
                new IdentityRole<Guid>(roleName));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error =>
                        $"{error.Code}: {error.Description}"));

                throw new InvalidOperationException(
                    $"Could not create role '{roleName}': {errors}");
            }
        }
    }
}
