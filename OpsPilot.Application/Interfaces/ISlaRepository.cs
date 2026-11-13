using OpsPilot.Domain.Entities;
using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.Interfaces;

public interface ISlaRepository
{
    Task<SlaPolicy?> GetActivePolicyAsync(
        IncidentPriority priority,
        CancellationToken cancellationToken = default);

    Task<IncidentSla?> GetByIncidentIdAsync(
        Guid incidentId,
        CancellationToken cancellationToken = default);

    Task AddIncidentSlaAsync(
        IncidentSla incidentSla,
        CancellationToken cancellationToken = default);
}
