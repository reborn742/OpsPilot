using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.DTOs.Assets;

public sealed record CreateAssetRequest(
    string AssetTag,
    string Name,
    AssetCategory Category,
    string? Manufacturer = null,
    string? Model = null,
    string? SerialNumber = null,
    DateOnly? PurchaseDate = null,
    DateOnly? WarrantyExpiryDate = null);
