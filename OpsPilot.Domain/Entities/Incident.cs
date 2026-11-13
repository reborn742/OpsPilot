using OpsPilot.Domain.Enums;

namespace OpsPilot.Domain.Entities;

public class Incident
{
    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public IncidentCategory Category { get; private set; }
    public IncidentPriority Priority { get; private set; }
    public IncidentStatus Status { get; private set; }

    public Guid? AssetId { get; private set; }
    public Guid? AssignedTechnicianId { get; private set; }

    public string? ResolutionNotes { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? ResolvedAtUtc { get; private set; }
    public DateTimeOffset? ClosedAtUtc { get; private set; }

    private Incident()
    {
    }

    public Incident(
        string title,
        string description,
        IncidentCategory category,
        IncidentPriority priority,
        Guid? assetId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Incident title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Incident description is required.", nameof(description));

        if (!Enum.IsDefined(category))
            throw new ArgumentOutOfRangeException(nameof(category));

        if (!Enum.IsDefined(priority))
            throw new ArgumentOutOfRangeException(nameof(priority));

        if (assetId == Guid.Empty)
            throw new ArgumentException("Asset ID cannot be empty.", nameof(assetId));

        Id = Guid.NewGuid();
        Title = title.Trim();
        Description = description.Trim();
        Category = category;
        Priority = priority;
        AssetId = assetId;

        Status = IncidentStatus.Open;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Assign(Guid technicianId)
    {
        if (technicianId == Guid.Empty)
            throw new ArgumentException("Technician ID is required.", nameof(technicianId));

        if (Status is not (
            IncidentStatus.Open or
            IncidentStatus.Assigned or
            IncidentStatus.InProgress))
        {
            throw new InvalidOperationException(
                "This incident cannot be assigned in its current status.");
        }

        AssignedTechnicianId = technicianId;

        if (Status == IncidentStatus.Open)
            Status = IncidentStatus.Assigned;
    }

    public void StartWork()
    {
        EnsureStatus(IncidentStatus.Assigned);
        Status = IncidentStatus.InProgress;
    }

    public void PutOnHold()
    {
        EnsureStatus(IncidentStatus.InProgress);
        Status = IncidentStatus.Pending;
    }

    public void ResumeWork()
    {
        EnsureStatus(IncidentStatus.Pending);
        Status = IncidentStatus.InProgress;
    }

    public void Resolve(string resolutionNotes)
    {
        EnsureStatus(IncidentStatus.InProgress);

        if (string.IsNullOrWhiteSpace(resolutionNotes))
            throw new ArgumentException(
                "Resolution notes are required.",
                nameof(resolutionNotes));

        ResolutionNotes = resolutionNotes.Trim();
        ResolvedAtUtc = DateTimeOffset.UtcNow;
        Status = IncidentStatus.Resolved;
    }

    public void Close()
    {
        EnsureStatus(IncidentStatus.Resolved);

        ClosedAtUtc = DateTimeOffset.UtcNow;
        Status = IncidentStatus.Closed;
    }

    public void Reopen()
    {
        if (Status is not (
            IncidentStatus.Resolved or
            IncidentStatus.Closed))
        {
            throw new InvalidOperationException(
                "Only resolved or closed incidents can be reopened.");
        }

        Status = IncidentStatus.InProgress;
        ResolutionNotes = null;
        ResolvedAtUtc = null;
        ClosedAtUtc = null;
    }

    private void EnsureStatus(IncidentStatus expectedStatus)
    {
        if (Status != expectedStatus)
        {
            throw new InvalidOperationException(
                $"This operation requires status {expectedStatus}. Current status: {Status}.");
        }
    }
}
