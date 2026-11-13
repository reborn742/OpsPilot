using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.DTOs.Incidents;

public sealed record CreateIncidentRequest(
    string Title,
    string Description,
    IncidentCategory Category,
    IncidentPriority Priority,
    Guid? AssetId = null);
