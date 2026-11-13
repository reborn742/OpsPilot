using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.DTOs.Incidents;

public sealed record IncidentDto(
    Guid Id,
    string Title,
    string Description,
    IncidentCategory Category,
    IncidentPriority Priority,
    IncidentStatus Status,
    Guid? AssetId,
    Guid? AssignedTechnicianId,
    string? ResolutionNotes,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ResolvedAtUtc,
    DateTimeOffset? ClosedAtUtc);
