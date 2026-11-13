using OpsPilot.Application.DTOs.Maintenance;

namespace OpsPilot.Application.Services.Maintenance;

public interface IMaintenanceService
{
    Task<List<MaintenanceDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<MaintenanceDto> CreateAsync(
        CreateMaintenanceRequest request,
        CancellationToken cancellationToken = default);

    Task<MaintenanceDto> StartAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<MaintenanceDto> CompleteAsync(
        Guid id,
        string? notes = null,
        CancellationToken cancellationToken = default);

    Task<MaintenanceDto> CancelAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
