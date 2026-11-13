using Microsoft.EntityFrameworkCore;
using OpsPilot.Application.DTOs.Dashboard;
using OpsPilot.Application.Services.Dashboard;
using OpsPilot.Domain.Enums;
using OpsPilot.Infrastructure.Persistence;

namespace OpsPilot.Infrastructure.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly OpsPilotDbContext _dbContext;

    public DashboardService(OpsPilotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardSummary> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var totalAssets = await _dbContext.Assets
            .CountAsync(cancellationToken);

        var activeAssets = await _dbContext.Assets
            .CountAsync(
                x => x.Status == AssetStatus.Active,
                cancellationToken);

        var openIncidents = await _dbContext.Incidents
            .CountAsync(
                x => x.Status != IncidentStatus.Resolved
                  && x.Status != IncidentStatus.Closed,
                cancellationToken);

        var criticalIncidents = await _dbContext.Incidents
            .CountAsync(
                x => x.Priority == IncidentPriority.Critical
                  && x.Status != IncidentStatus.Resolved
                  && x.Status != IncidentStatus.Closed,
                cancellationToken);

        var pendingIncidents = await _dbContext.Incidents
            .CountAsync(
                x => x.Status == IncidentStatus.Pending,
                cancellationToken);

        return new DashboardSummary(
            totalAssets,
            activeAssets,
            openIncidents,
            criticalIncidents,
            pendingIncidents);
    }
}
