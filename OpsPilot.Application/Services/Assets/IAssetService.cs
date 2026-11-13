using OpsPilot.Application.Common;
using OpsPilot.Application.DTOs.Assets;
using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.Services.Assets;

public interface IAssetService
{
    Task<AssetDto> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken = default);
    Task<AssetDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<AssetDto>> SearchAsync(AssetSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<AssetDto> AssignAsync(Guid id, Guid? userId, CancellationToken cancellationToken = default);
    Task<AssetDto> UpdateConditionAsync(Guid id, AssetCondition condition, CancellationToken cancellationToken = default);
    Task<AssetDto> MarkForRepairAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AssetDto> MarkActiveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AssetDto> RetireAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AssetDto> DisposeAsync(Guid id, CancellationToken cancellationToken = default);
}
