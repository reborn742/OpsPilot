using OpsPilot.Application.Common;
using OpsPilot.Application.DTOs.Incidents;
using OpsPilot.Application.Interfaces;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Application.Services.Incidents;

public sealed class IncidentService : IIncidentService
{
    private readonly IIncidentRepository _incidents;
    private readonly IAssetRepository _assets;
    private readonly ITechnicianDirectory _technicians;
    private readonly ISlaRepository _slas;
    private readonly IAuditLogWriter _audit;
    private readonly IUnitOfWork _unitOfWork;

    public IncidentService(
        IIncidentRepository incidents,
        IAssetRepository assets,
        ITechnicianDirectory technicians,
        ISlaRepository slas,
        IAuditLogWriter audit,
        IUnitOfWork unitOfWork)
    {
        _incidents = incidents;
        _assets = assets;
        _technicians = technicians;
        _slas = slas;
        _audit = audit;
        _unitOfWork = unitOfWork;
    }

    public async Task<IncidentDto> CreateAsync(
        CreateIncidentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.AssetId.HasValue &&
            await _assets.GetByIdAsync(
                request.AssetId.Value, cancellationToken) is null)
        {
            throw new KeyNotFoundException(
                $"Asset '{request.AssetId}' was not found.");
        }

        var policy = await _slas.GetActivePolicyAsync(
            request.Priority, cancellationToken)
            ?? throw new InvalidOperationException(
                $"No active SLA policy exists for priority '{request.Priority}'.");

        var incident = new Incident(
            request.Title,
            request.Description,
            request.Category,
            request.Priority,
            request.AssetId);

        var incidentSla = new IncidentSla(
            incident.Id,
            incident.Priority,
            policy.ResponseTargetMinutes,
            policy.ResolutionTargetMinutes,
            incident.CreatedAtUtc);

        await _incidents.AddAsync(incident, cancellationToken);
        await _slas.AddIncidentSlaAsync(incidentSla, cancellationToken);

        await _audit.WriteAsync(
            "Created",
            "Incident",
            incident.Id.ToString(),
            $"Priority: {incident.Priority}",
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(incident);
    }

    public async Task<IncidentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var incident = await _incidents.GetByIdAsync(id, cancellationToken);
        return incident is null ? null : Map(incident);
    }

    public async Task<PagedResult<IncidentDto>> SearchAsync(
        IncidentSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        var result = await _incidents.SearchAsync(
            criteria, cancellationToken);

        return new PagedResult<IncidentDto>(
            result.Items.Select(Map).ToList(),
            result.PageNumber,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<IncidentDto> AssignAsync(
        Guid id,
        Guid technicianId,
        CancellationToken cancellationToken = default)
    {
        if (!await _technicians.IsActiveTechnicianAsync(
            technicianId, cancellationToken))
        {
            throw new InvalidOperationException(
                "Assignment requires an active user with the Technician role.");
        }

        var incident = await GetRequiredIncidentAsync(id, cancellationToken);
        incident.Assign(technicianId);

        var sla = await GetRequiredSlaAsync(id, cancellationToken);
        sla.RecordFirstResponse(DateTimeOffset.UtcNow);

        await _audit.WriteAsync(
            "Assigned",
            "Incident",
            incident.Id.ToString(),
            "Technician assignment updated.",
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(incident);
    }

    public Task<IncidentDto> StartWorkAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id, incident => incident.StartWork(),
            "WorkStarted", cancellationToken);

    public Task<IncidentDto> PutOnHoldAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id, incident => incident.PutOnHold(),
            "PutOnHold", cancellationToken);

    public Task<IncidentDto> ResumeWorkAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id, incident => incident.ResumeWork(),
            "WorkResumed", cancellationToken);

    public async Task<IncidentDto> ResolveAsync(
        Guid id,
        string resolutionNotes,
        CancellationToken cancellationToken = default)
    {
        var incident = await GetRequiredIncidentAsync(id, cancellationToken);
        incident.Resolve(resolutionNotes);

        var sla = await GetRequiredSlaAsync(id, cancellationToken);

        sla.RecordResolution(
            incident.ResolvedAtUtc
            ?? throw new InvalidOperationException(
                "The incident resolution timestamp was not recorded."));

        await _audit.WriteAsync(
            "Resolved",
            "Incident",
            incident.Id.ToString(),
            "Resolution notes recorded.",
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(incident);
    }

    public Task<IncidentDto> CloseAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id, incident => incident.Close(),
            "Closed", cancellationToken);

    public async Task<IncidentDto> ReopenAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var incident = await GetRequiredIncidentAsync(id, cancellationToken);
        incident.Reopen();

        var sla = await GetRequiredSlaAsync(id, cancellationToken);
        sla.ReopenResolutionClock();

        await _audit.WriteAsync(
            "Reopened",
            "Incident",
            incident.Id.ToString(),
            "Incident reopened.",
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(incident);
    }

    private async Task<Incident> GetRequiredIncidentAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _incidents.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Incident '{id}' was not found.");
    }

    private async Task<IncidentSla> GetRequiredSlaAsync(
        Guid incidentId,
        CancellationToken cancellationToken)
    {
        return await _slas.GetByIncidentIdAsync(
            incidentId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"No SLA record exists for incident '{incidentId}'.");
    }

    private async Task<IncidentDto> UpdateAsync(
        Guid id,
        Action<Incident> update,
        string auditAction,
        CancellationToken cancellationToken)
    {
        var incident = await GetRequiredIncidentAsync(id, cancellationToken);
        update(incident);

        await _audit.WriteAsync(
            auditAction,
            "Incident",
            incident.Id.ToString(),
            null,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(incident);
    }

    private static IncidentDto Map(Incident incident) => new(
        incident.Id,
        incident.Title,
        incident.Description,
        incident.Category,
        incident.Priority,
        incident.Status,
        incident.AssetId,
        incident.AssignedTechnicianId,
        incident.ResolutionNotes,
        incident.CreatedAtUtc,
        incident.ResolvedAtUtc,
        incident.ClosedAtUtc);
}
