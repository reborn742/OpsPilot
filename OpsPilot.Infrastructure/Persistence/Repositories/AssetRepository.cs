using Microsoft.EntityFrameworkCore;
using OpsPilot.Application.Common;
using OpsPilot.Application.DTOs.Assets;
using OpsPilot.Application.Interfaces;
using OpsPilot.Domain.Entities;
using OpsPilot.Infrastructure.Persistence;

namespace OpsPilot.Infrastructure.Persistence.Repositories;

public sealed class AssetRepository : IAssetRepository
{
    private readonly OpsPilotDbContext _context;

    public AssetRepository(OpsPilotDbContext context)
    {
        _context = context;
    }

    public Task<Asset?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Assets
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Asset?> GetByAssetTagAsync(
        string assetTag,
        CancellationToken cancellationToken = default)
    {
        return _context.Assets.FirstOrDefaultAsync(
            x => x.AssetTag == assetTag.Trim().ToUpper(),
            cancellationToken);
    }

    public async Task<PagedResult<Asset>> SearchAsync(
        AssetSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        ValidatePagination(criteria.PageNumber, criteria.PageSize);

        IQueryable<Asset> query = _context.Assets.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var term = criteria.SearchTerm.Trim();

            query = query.Where(x =>
                x.AssetTag.Contains(term) ||
                x.Name.Contains(term) ||
                (x.SerialNumber != null && x.SerialNumber.Contains(term)));
        }

        if (criteria.Category.HasValue)
            query = query.Where(x => x.Category == criteria.Category.Value);

        if (criteria.Status.HasValue)
            query = query.Where(x => x.Status == criteria.Status.Value);

        if (criteria.AssignedToUserId.HasValue)
            query = query.Where(
                x => x.AssignedToUserId == criteria.AssignedToUserId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.AssetTag)
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Asset>(
            items,
            criteria.PageNumber,
            criteria.PageSize,
            totalCount);
    }

    public async Task AddAsync(
        Asset asset,
        CancellationToken cancellationToken = default)
    {
        await _context.Assets.AddAsync(asset, cancellationToken);
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
