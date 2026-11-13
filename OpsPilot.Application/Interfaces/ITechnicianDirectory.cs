using OpsPilot.Application.DTOs.Technicians;

namespace OpsPilot.Application.Interfaces;

public interface ITechnicianDirectory
{
    Task<IReadOnlyList<TechnicianDto>> GetActiveTechniciansAsync(
        CancellationToken cancellationToken = default);

    Task<bool> IsActiveTechnicianAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
