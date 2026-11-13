using OpsPilot.Application.Common;
using OpsPilot.Application.DTOs.Incidents;

namespace OpsPilot.Application.Services.Incidents;

public interface IIncidentService
{
    Task<IncidentDto> CreateAsync(CreateIncidentRequest request, CancellationToken cancellationToken = default);
    Task<IncidentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<IncidentDto>> SearchAsync(IncidentSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<IncidentDto> AssignAsync(Guid id, Guid technicianId, CancellationToken cancellationToken = default);
    Task<IncidentDto> StartWorkAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IncidentDto> PutOnHoldAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IncidentDto> ResumeWorkAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IncidentDto> ResolveAsync(Guid id, string resolutionNotes, CancellationToken cancellationToken = default);
    Task<IncidentDto> CloseAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IncidentDto> ReopenAsync(Guid id, CancellationToken cancellationToken = default);
}
