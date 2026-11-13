using OpsPilot.Application.Common;

namespace OpsPilot.Application.Interfaces;

public interface ICurrentUserContext
{
    Task<AuditActor> GetCurrentActorAsync(
        CancellationToken cancellationToken = default);
}
