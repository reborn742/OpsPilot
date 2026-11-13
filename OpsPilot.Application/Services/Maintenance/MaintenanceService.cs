using OpsPilot.Application.DTOs.Maintenance;
using OpsPilot.Application.Interfaces;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Application.Services.Maintenance;

public sealed class MaintenanceService : IMaintenanceService
{
    private readonly IMaintenanceRepository _maintenance;
    private readonly IAssetRepository _assets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogWriter _auditLog;

    public MaintenanceService(
        IMaintenanceRepository maintenance,
        IAssetRepository assets,
        IUnitOfWork unitOfWork,
        IAuditLogWriter auditLog)
    {
        _maintenance = maintenance;
        _assets = assets;
        _unitOfWork = unitOfWork;
        _auditLog = auditLog;
    }

    public async Task<List<MaintenanceDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var records = await _maintenance.GetAllAsync(cancellationToken);
        return records.Select(Map).ToList();
    }

    public async Task<MaintenanceDto> CreateAsync(
        CreateMaintenanceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var asset = await _assets.GetByIdAsync(
            request.AssetId, cancellationToken)
            ?? throw new KeyNotFoundException("Asset was not found.");

        var record = new MaintenanceRecord(
            request.AssetId,
            request.Title,
            request.ScheduledAtUtc,
            request.Description);

        await _maintenance.AddAsync(record, cancellationToken);

        await _auditLog.WriteAsync(
            "Scheduled",
            "Maintenance",
            record.Id.ToString(),
            $"Maintenance scheduled for asset {asset.AssetTag}.",
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(record, asset.AssetTag, asset.Name);
    }

    public Task<MaintenanceDto> StartAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id,
            record => record.Start(),
            "Started",
            "Maintenance work started.",
            cancellationToken);

    public Task<MaintenanceDto> CompleteAsync(
        Guid id,
        string? notes = null,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id,
            record => record.Complete(notes),
            "Completed",
            "Maintenance completed.",
            cancellationToken);

    public Task<MaintenanceDto> CancelAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => UpdateAsync(
            id,
            record => record.Cancel(),
            "Cancelled",
            "Maintenance cancelled.",
            cancellationToken);

    private async Task<MaintenanceDto> UpdateAsync(
        Guid id,
        Action<MaintenanceRecord> update,
        string action,
        string? details,
        CancellationToken cancellationToken)
    {
        var record = await _maintenance.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Maintenance record '{id}' was not found.");

        update(record);

        await _auditLog.WriteAsync(
            action,
            "Maintenance",
            record.Id.ToString(),
            details,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(record);
    }

    private static MaintenanceDto Map(MaintenanceRecord record) =>
        Map(
            record,
            record.Asset?.AssetTag ?? "Unknown",
            record.Asset?.Name ?? "Unknown");

    private static MaintenanceDto Map(
        MaintenanceRecord record,
        string assetTag,
        string assetName) =>
        new(
            record.Id,
            record.AssetId,
            assetTag,
            assetName,
            record.Title,
            record.Description,
            record.ScheduledAtUtc,
            record.CompletedAtUtc,
            record.CompletionNotes,
            record.Status,
            record.CreatedAtUtc);
}
