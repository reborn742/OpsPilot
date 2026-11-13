using OpsPilot.Application.Common;
using OpsPilot.Application.DTOs.Incidents;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Application.Interfaces;

public interface IIncidentRepository
{
    Task<Incident?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Incident>> SearchAsync(
        IncidentSearchCriteria criteria,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Incident incident,
        CancellationToken cancellationToken = default);
}
