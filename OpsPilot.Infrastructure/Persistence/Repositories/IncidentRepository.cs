using Microsoft.EntityFrameworkCore;
using OpsPilot.Application.Common;
using OpsPilot.Application.DTOs.Incidents;
using OpsPilot.Application.Interfaces;
using OpsPilot.Domain.Entities;
using OpsPilot.Infrastructure.Persistence;

namespace OpsPilot.Infrastructure.Persistence.Repositories;

public sealed class IncidentRepository : IIncidentRepository
{
    private readonly OpsPilotDbContext _context;

    public IncidentRepository(OpsPilotDbContext context)
    {
        _context = context;
    }

    public Task<Incident?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Incidents
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Incident>> SearchAsync(
        IncidentSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        ValidatePagination(criteria.PageNumber, criteria.PageSize);

        IQueryable<Incident> query = _context.Incidents.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var term = criteria.SearchTerm.Trim();

            query = query.Where(x =>
                x.Title.Contains(term) ||
                x.Description.Contains(term));
        }

        if (criteria.Category.HasValue)
            query = query.Where(x => x.Category == criteria.Category.Value);

        if (criteria.Priority.HasValue)
            query = query.Where(x => x.Priority == criteria.Priority.Value);

        if (criteria.Status.HasValue)
            query = query.Where(x => x.Status == criteria.Status.Value);

        if (criteria.AssignedTechnicianId.HasValue)
            query = query.Where(
                x => x.AssignedTechnicianId == criteria.AssignedTechnicianId.Value);

        if (criteria.AssetId.HasValue)
            query = query.Where(x => x.AssetId == criteria.AssetId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenBy(x => x.Id)
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Incident>(
            items,
            criteria.PageNumber,
            criteria.PageSize,
            totalCount);
    }

    public async Task AddAsync(
        Incident incident,
        CancellationToken cancellationToken = default)
    {
        await _context.Incidents.AddAsync(incident, cancellationToken);
    }

    private static void ValidatePagination(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize is < 1 or > 100)
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be between 1 and 100.");
    }
}
