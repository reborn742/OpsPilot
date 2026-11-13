using OpsPilot.Application.DTOs.Maintenance;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Application.Interfaces;

public interface IMaintenanceRepository
{
    Task<List<MaintenanceRecord>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<MaintenanceRecord?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        MaintenanceRecord record,
        CancellationToken cancellationToken = default);
}
