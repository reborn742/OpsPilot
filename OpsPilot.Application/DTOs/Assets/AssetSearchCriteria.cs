using OpsPilot.Domain.Enums;

namespace OpsPilot.Application.DTOs.Assets;

public sealed record AssetSearchCriteria(
    string? SearchTerm = null,
    AssetCategory? Category = null,
    AssetStatus? Status = null,
    Guid? AssignedToUserId = null,
    int PageNumber = 1,
    int PageSize = 20);
