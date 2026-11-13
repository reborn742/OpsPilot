using Microsoft.EntityFrameworkCore;
using OpsPilot.Infrastructure.Identity;
using OpsPilot.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using OpsPilot.Infrastructure;
using OpsPilot.Infrastructure.Services;
using OpsPilot.Web.Components;
using OpsPilot.Web.Services;
using OpsPilot.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("OpsPilotDb")
    ?? throw new InvalidOperationException("OpsPilotDb connection string is missing."));
builder.Services.AddHostedService<SlaBreachMonitor>();

builder.Services
    .AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<ICurrentUserContext, BlazorCurrentUserContext>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var migrationScope = app.Services.CreateAsyncScope();
    await migrationScope.ServiceProvider
        .GetRequiredService<OpsPilotDbContext>()
        .Database.MigrateAsync();
}

await IdentitySeeder.SeedRolesAsync(app.Services);

if (args.Contains("--create-superadmin"))
{
    await using var scope = app.Services.CreateAsyncScope();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();

    var existingAdmins = await userManager.GetUsersInRoleAsync(
        RoleNames.SuperAdmin);

    if (existingAdmins.Count > 0)
    {
        Console.WriteLine("A SuperAdmin already exists. Setup stopped.");
        return;
    }

    Console.Write("Full name: ");
    var fullName = Console.ReadLine()?.Trim();

    Console.Write("Email: ");
    var email = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(fullName) ||
        string.IsNullOrWhiteSpace(email))
    {
        Console.WriteLine("Name and email are required.");
        return;
    }

    Console.Write("Password: ");
    var password = ReadPassword();

    Console.WriteLine();
    Console.Write("Confirm password: ");
    var confirmation = ReadPassword();

    Console.WriteLine();

    if (password.Length == 0 || password != confirmation)
    {
        Console.WriteLine("Password is empty or passwords do not match.");
        return;
    }

    var user = new ApplicationUser
    {
        UserName = email,
        Email = email,
        FullName = fullName,
        IsActive = true
    };

    var createResult = await userManager.CreateAsync(user, password);

    if (!createResult.Succeeded)
    {
        foreach (var error in createResult.Errors)
            Console.WriteLine($"{error.Code}: {error.Description}");

        return;
    }

    var roleResult = await userManager.AddToRoleAsync(
        user,
        RoleNames.SuperAdmin);

    if (!roleResult.Succeeded)
    {
        await userManager.DeleteAsync(user);

        foreach (var error in roleResult.Errors)
            Console.WriteLine($"{error.Code}: {error.Description}");

        Console.WriteLine("Account creation rolled back.");
        return;
    }

    Console.WriteLine($"SuperAdmin created successfully: {email}");
    return;
}

if (app.Environment.IsDevelopment())
{
    await DemoDataSeeder.SeedAsync(app.Services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


static string ReadPassword()
{
    var password = new System.Text.StringBuilder();

    while (true)
    {
        var key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Enter)
            break;

        if (key.Key == ConsoleKey.Backspace)
        {
            if (password.Length > 0)
            {
                password.Length--;
                Console.Write("\b \b");
            }

            continue;
        }

        if (!char.IsControl(key.KeyChar))
        {
            password.Append(key.KeyChar);
            Console.Write("*");
        }
    }

    return password.ToString();
}
