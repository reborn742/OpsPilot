using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.DTOs.Incidents;

public sealed record IncidentSearchCriteria(
    string? SearchTerm = null,
    IncidentCategory? Category = null,
    IncidentPriority? Priority = null,
    IncidentStatus? Status = null,
    Guid? AssignedTechnicianId = null,
    Guid? AssetId = null,
    int PageNumber = 1,
    int PageSize = 20);
