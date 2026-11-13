namespace OpsPilot.Application.DTOs.Maintenance;

public sealed record CreateMaintenanceRequest(
    Guid AssetId,
    string Title,
    DateTimeOffset ScheduledAtUtc,
    string? Description = null);
