using OpsPilot.Application.Common;
using OpsPilot.Application.DTOs.Assets;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Application.Interfaces;

public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Asset?> GetByAssetTagAsync(
        string assetTag,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Asset>> SearchAsync(
        AssetSearchCriteria criteria,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Asset asset,
        CancellationToken cancellationToken = default);
}
