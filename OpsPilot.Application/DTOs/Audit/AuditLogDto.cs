namespace OpsPilot.Application.DTOs.Audit;

public sealed record AuditLogDto(
    Guid Id,
    Guid? ActorUserId,
    string ActorDisplayName,
    string Action,
    string EntityType,
    string EntityId,
    string? Details,
    DateTimeOffset OccurredAtUtc);
