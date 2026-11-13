using OpsPilot.Application.Common;
using OpsPilot.Application.DTOs.Audit;

namespace OpsPilot.Application.Interfaces;

public interface IAuditLogQueryService
{
    Task<PagedResult<AuditLogDto>> SearchAsync(
        AuditLogSearchCriteria criteria,
        CancellationToken cancellationToken = default);
}
