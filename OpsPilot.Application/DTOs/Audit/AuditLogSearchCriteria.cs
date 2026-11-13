namespace OpsPilot.Application.DTOs.Audit;

public sealed class AuditLogSearchCriteria
{
    public string? EntityType { get; set; }
    public string? Action { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
