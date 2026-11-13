namespace OpsPilot.Domain.Entities;

public enum MaintenanceStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public sealed class MaintenanceRecord
{
    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public Asset? Asset { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTimeOffset ScheduledAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public string? CompletionNotes { get; private set; }
    public MaintenanceStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    private MaintenanceRecord() { }

    public MaintenanceRecord(
        Guid assetId,
        string title,
        DateTimeOffset scheduledAtUtc,
        string? description = null)
    {
        if (assetId == Guid.Empty)
            throw new ArgumentException("Asset ID is required.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Maintenance title is required.");

        if (title.Trim().Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters.");

        Id = Guid.NewGuid();
        AssetId = assetId;
        Title = title.Trim();
        Description = Normalize(description);
        ScheduledAtUtc = scheduledAtUtc;
        Status = MaintenanceStatus.Scheduled;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Start()
    {
        if (Status != MaintenanceStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled maintenance can start.");

        Status = MaintenanceStatus.InProgress;
    }

    public void Complete(string? notes = null)
    {
        if (Status != MaintenanceStatus.InProgress)
            throw new InvalidOperationException("Only in-progress maintenance can be completed.");

        if (notes?.Length > 2000)
            throw new ArgumentException("Completion notes cannot exceed 2000 characters.");

        Status = MaintenanceStatus.Completed;
        CompletedAtUtc = DateTimeOffset.UtcNow;
        CompletionNotes = Normalize(notes);
    }

    public void Cancel()
    {
        if (Status is MaintenanceStatus.Completed or MaintenanceStatus.Cancelled)
            throw new InvalidOperationException("This maintenance record cannot be cancelled.");

        Status = MaintenanceStatus.Cancelled;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
