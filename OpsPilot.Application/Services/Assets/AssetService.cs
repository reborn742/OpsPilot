using OpsPilot.Application.Common;
using OpsPilot.Application.DTOs.Assets;
using OpsPilot.Application.Interfaces;
using OpsPilot.Domain.Entities;
using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.Services.Assets;

public sealed class AssetService : IAssetService
{
    private readonly IAssetRepository _assets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogWriter _auditLog;

    public AssetService(
        IAssetRepository assets,
        IUnitOfWork unitOfWork,
        IAuditLogWriter auditLog)
    {
        _assets = assets;
        _unitOfWork = unitOfWork;
        _auditLog = auditLog;
    }

    public async Task<AssetDto> CreateAsync(
        CreateAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await _assets.GetByAssetTagAsync(request.AssetTag, cancellationToken) is not null)
            throw new InvalidOperationException(
                $"Asset tag '{request.AssetTag}' already exists.");

        var asset = new Asset(
            request.AssetTag,
            request.Name,
            request.Category,
            request.Manufacturer,
            request.Model,
            request.SerialNumber,
            request.PurchaseDate,
            request.WarrantyExpiryDate);

        await _assets.AddAsync(asset, cancellationToken);

        await _auditLog.WriteAsync(
            "Created",
            "Asset",
            asset.Id.ToString(),
            $"Asset tag: {asset.AssetTag}",
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(asset);
    }

    public async Task<AssetDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assets.GetByIdAsync(id, cancellationToken);
        return asset is null ? null : Map(asset);
    }

    public async Task<PagedResult<AssetDto>> SearchAsync(
        AssetSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        var result = await _assets.SearchAsync(criteria, cancellationToken);

        return new PagedResult<AssetDto>(
            result.Items.Select(Map).ToList(),
            result.PageNumber,
            result.PageSize,
            result.TotalCount);
    }

    public Task<AssetDto> AssignAsync(
        Guid id,
        Guid? userId,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id,
            asset => asset.AssignTo(userId),
            "AssignmentChanged",
            userId.HasValue ? "Asset assigned." : "Asset unassigned.",
            cancellationToken);

    public Task<AssetDto> UpdateConditionAsync(
        Guid id,
        AssetCondition condition,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id,
            asset => asset.UpdateCondition(condition),
            "ConditionUpdated",
            $"Condition: {condition}",
            cancellationToken);

    public Task<AssetDto> MarkForRepairAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id,
            asset => asset.MarkForRepair(),
            "MarkedForRepair",
            null,
            cancellationToken);

    public Task<AssetDto> MarkActiveAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id,
            asset => asset.MarkActive(),
            "MarkedActive",
            null,
            cancellationToken);

    public Task<AssetDto> RetireAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id,
            asset => asset.Retire(),
            "Retired",
            null,
            cancellationToken);

    public Task<AssetDto> DisposeAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id,
            asset => asset.Dispose(),
            "Disposed",
            null,
            cancellationToken);

    private async Task<AssetDto> UpdateAsync(
        Guid id,
        Action<Asset> update,
        string auditAction,
        string? details,
        CancellationToken cancellationToken)
    {
        var asset = await _assets.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset '{id}' was not found.");

        update(asset);

        await _auditLog.WriteAsync(
            auditAction,
            "Asset",
            asset.Id.ToString(),
            details,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(asset);
    }

    private static AssetDto Map(Asset asset) => new(
        asset.Id,
        asset.AssetTag,
        asset.Name,
        asset.Category,
        asset.Status,
        asset.Condition,
        asset.Manufacturer,
        asset.Model,
        asset.SerialNumber,
        asset.PurchaseDate,
        asset.WarrantyExpiryDate,
        asset.AssignedToUserId,
        asset.CreatedAtUtc);
}
