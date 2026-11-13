namespace OpsPilot.Application.Interfaces;

public interface IAuditLogWriter
{
    Task WriteAsync(
        string action,
        string entityType,
        string entityId,
        string? details = null,
        CancellationToken cancellationToken = default);
}
