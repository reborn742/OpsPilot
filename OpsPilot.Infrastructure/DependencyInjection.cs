using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpsPilot.Application.Interfaces;
using OpsPilot.Application.Services.Assets;
using OpsPilot.Application.Services.Dashboard;
using OpsPilot.Application.Services.Incidents;
using OpsPilot.Application.Services.Maintenance;
using OpsPilot.Infrastructure.Identity;
using OpsPilot.Infrastructure.Persistence;
using OpsPilot.Infrastructure.Persistence.Repositories;
using OpsPilot.Infrastructure.Services;

namespace OpsPilot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<OpsPilotDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 12;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<OpsPilotDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<ISlaRepository, SlaRepository>();
        services.AddScoped<IAuditLogWriter, AuditLogWriter>();
        services.AddScoped<IAuditLogQueryService, AuditLogQueryService>();
        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<OpsPilotDbContext>());

        services.AddScoped<IAssetService, AssetService>();
        services.AddScoped<IIncidentService, IncidentService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<ITechnicianDirectory, TechnicianDirectory>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
