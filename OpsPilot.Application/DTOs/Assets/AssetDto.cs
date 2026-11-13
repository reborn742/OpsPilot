using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.DTOs.Assets;

public sealed record AssetDto(
    Guid Id,
    string AssetTag,
    string Name,
    AssetCategory Category,
    AssetStatus Status,
    AssetCondition Condition,
    string? Manufacturer,
    string? Model,
    string? SerialNumber,
    DateOnly? PurchaseDate,
    DateOnly? WarrantyExpiryDate,
    Guid? AssignedToUserId,
    DateTimeOffset CreatedAtUtc);
