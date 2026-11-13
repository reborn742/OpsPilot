using Microsoft.EntityFrameworkCore;
using OpsPilot.Application.Common;
using OpsPilot.Application.DTOs.Audit;
using OpsPilot.Application.Interfaces;
using OpsPilot.Infrastructure.Persistence;

namespace OpsPilot.Infrastructure.Services;

public sealed class AuditLogQueryService : IAuditLogQueryService
{
    private readonly OpsPilotDbContext _db;

    public AuditLogQueryService(OpsPilotDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<AuditLogDto>> SearchAsync(
        AuditLogSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        var pageNumber = Math.Max(1, criteria.PageNumber);
        var pageSize = Math.Clamp(criteria.PageSize, 1, 100);

        var query = _db.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(criteria.EntityType))
        {
            var entityType = criteria.EntityType.Trim();
            query = query.Where(x => x.EntityType == entityType);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Action))
        {
            var action = criteria.Action.Trim();
            query = query.Where(x => x.Action == action);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var term = criteria.SearchTerm.Trim();

            query = query.Where(x =>
                x.ActorDisplayName.Contains(term) ||
                x.EntityId.Contains(term) ||
                (x.Details != null && x.Details.Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.OccurredAtUtc)
            .ThenByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AuditLogDto(
                x.Id,
                x.ActorUserId,
                x.ActorDisplayName,
                x.Action,
                x.EntityType,
                x.EntityId,
                x.Details,
                x.OccurredAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLogDto>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }
}
