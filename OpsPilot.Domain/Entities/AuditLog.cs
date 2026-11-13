namespace OpsPilot.Domain.Entities;

public sealed class AuditLog
{
    public Guid Id { get; private set; }

    public Guid? ActorUserId { get; private set; }
    public string ActorDisplayName { get; private set; } = string.Empty;

    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;

    public string? Details { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    private AuditLog()
    {
    }

    public AuditLog(
        Guid? actorUserId,
        string actorDisplayName,
        string action,
        string entityType,
        string entityId,
        string? details = null)
    {
        if (string.IsNullOrWhiteSpace(actorDisplayName))
            throw new ArgumentException("Actor display name is required.");

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Audit action is required.");

        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type is required.");

        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException("Entity ID is required.");

        Id = Guid.NewGuid();
        ActorUserId = actorUserId;
        ActorDisplayName = actorDisplayName.Trim();
        Action = action.Trim();
        EntityType = entityType.Trim();
        EntityId = entityId.Trim();
        Details = string.IsNullOrWhiteSpace(details) ? null : details.Trim();
        OccurredAtUtc = DateTimeOffset.UtcNow;
    }
}
