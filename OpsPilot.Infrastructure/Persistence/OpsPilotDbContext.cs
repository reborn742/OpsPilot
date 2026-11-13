using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpsPilot.Application.Interfaces;
using OpsPilot.Domain.Entities;
using OpsPilot.Infrastructure.Identity;


namespace OpsPilot.Infrastructure.Persistence;

public sealed class OpsPilotDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IUnitOfWork
{
    public OpsPilotDbContext(DbContextOptions<OpsPilotDbContext> options)
        : base(options)
    {
    }

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<Incident> Incidents => Set<Incident>();

public DbSet<SlaPolicy> SlaPolicies => Set<SlaPolicy>();

public DbSet<IncidentSla> IncidentSlas => Set<IncidentSla>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OpsPilotDbContext).Assembly);
    }
}
