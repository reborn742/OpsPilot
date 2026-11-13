using OpsPilot.Application.Interfaces;
using OpsPilot.Domain.Entities;
using OpsPilot.Infrastructure.Persistence;

namespace OpsPilot.Infrastructure.Services;

public sealed class AuditLogWriter : IAuditLogWriter
{
    private readonly OpsPilotDbContext _db;
    private readonly ICurrentUserContext _currentUser;

    public AuditLogWriter(
        OpsPilotDbContext db,
        ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task WriteAsync(
        string action,
        string entityType,
        string entityId,
        string? details = null,
        CancellationToken cancellationToken = default)
    {
        var actor = await _currentUser.GetCurrentActorAsync(
            cancellationToken);

        var entry = new AuditLog(
            actor.UserId,
            actor.DisplayName,
            action,
            entityType,
            entityId,
            details);

        await _db.AuditLogs.AddAsync(entry, cancellationToken);
    }
}
