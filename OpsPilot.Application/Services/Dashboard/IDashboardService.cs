using OpsPilot.Application.DTOs.Dashboard;

namespace OpsPilot.Application.Services.Dashboard;

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync(
        CancellationToken cancellationToken = default);
}
