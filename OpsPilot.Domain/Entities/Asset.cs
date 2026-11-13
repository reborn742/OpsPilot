using OpsPilot.Domain.Enums;

namespace OpsPilot.Domain.Entities;

public class Asset
{
    public Guid Id { get; private set; }

    public string AssetTag { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;

    public string? Manufacturer { get; private set; }
    public string? Model { get; private set; }
    public string? SerialNumber { get; private set; }

    public AssetCategory Category { get; private set; }
    public AssetStatus Status { get; private set; }
    public AssetCondition Condition { get; private set; }

    public DateOnly? PurchaseDate { get; private set; }
    public DateOnly? WarrantyExpiryDate { get; private set; }

    public Guid? AssignedToUserId { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    private Asset()
    {
    }

    public Asset(
        string assetTag,
        string name,
        AssetCategory category,
        string? manufacturer = null,
        string? model = null,
        string? serialNumber = null,
        DateOnly? purchaseDate = null,
        DateOnly? warrantyExpiryDate = null)
    {
        if (string.IsNullOrWhiteSpace(assetTag))
            throw new ArgumentException("Asset tag is required.", nameof(assetTag));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Asset name is required.", nameof(name));

        if (!Enum.IsDefined(category))
            throw new ArgumentOutOfRangeException(nameof(category));

        if (purchaseDate.HasValue &&
            warrantyExpiryDate.HasValue &&
            warrantyExpiryDate < purchaseDate)
        {
            throw new ArgumentException(
                "Warranty expiry cannot be earlier than purchase date.");
        }

        Id = Guid.NewGuid();
        AssetTag = assetTag.Trim().ToUpperInvariant();
        Name = name.Trim();
        Category = category;
        Manufacturer = NormalizeOptional(manufacturer);
        Model = NormalizeOptional(model);
        SerialNumber = NormalizeOptional(serialNumber);
        PurchaseDate = purchaseDate;
        WarrantyExpiryDate = warrantyExpiryDate;

        Status = AssetStatus.Active;
        Condition = AssetCondition.Good;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void AssignTo(Guid? userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        if (Status is AssetStatus.Retired or AssetStatus.Disposed)
            throw new InvalidOperationException(
                "Retired or disposed assets cannot be assigned.");

        AssignedToUserId = userId;
    }

    public void UpdateCondition(AssetCondition condition)
    {
        if (!Enum.IsDefined(condition))
            throw new ArgumentOutOfRangeException(nameof(condition));

        Condition = condition;
    }

    public void MarkForRepair()
    {
        if (Status is AssetStatus.Retired or AssetStatus.Disposed)
            throw new InvalidOperationException(
                "Retired or disposed assets cannot enter repair.");

        Status = AssetStatus.InRepair;
    }

    public void MarkActive()
    {
        if (Status is AssetStatus.Retired or AssetStatus.Disposed)
            throw new InvalidOperationException(
                "Retired or disposed assets cannot be reactivated.");

        Status = AssetStatus.Active;
    }

    public void Retire()
    {
        Status = AssetStatus.Retired;
        AssignedToUserId = null;
    }

    public void Dispose()
    {
        Status = AssetStatus.Disposed;
        AssignedToUserId = null;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
