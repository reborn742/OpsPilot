namespace OpsPilot.Application.Common;

public sealed record AuditActor(
    Guid? UserId,
    string DisplayName);
