using OpsPilot.Domain.Entities;

namespace OpsPilot.Application.DTOs.Maintenance;

public sealed record MaintenanceDto(
    Guid Id,
    Guid AssetId,
    string AssetTag,
    string AssetName,
    string Title,
    string? Description,
    DateTimeOffset ScheduledAtUtc,
    DateTimeOffset? CompletedAtUtc,
    string? CompletionNotes,
    MaintenanceStatus Status,
    DateTimeOffset CreatedAtUtc);
