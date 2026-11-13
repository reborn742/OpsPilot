using OpsPilot.Domain.Enums;

namespace OpsPilot.Domain.Entities;

public sealed class IncidentSla
{
    public Guid Id { get; private set; }
    public Guid IncidentId { get; private set; }
    public IncidentPriority Priority { get; private set; }

    public int ResponseTargetMinutes { get; private set; }
    public int ResolutionTargetMinutes { get; private set; }

    public DateTimeOffset ResponseDueAtUtc { get; private set; }
    public DateTimeOffset ResolutionDueAtUtc { get; private set; }

    public DateTimeOffset? FirstResponseAtUtc { get; private set; }
    public DateTimeOffset? ResolvedAtUtc { get; private set; }

    public DateTimeOffset? ResponseBreachedAtUtc { get; private set; }
    public DateTimeOffset? ResolutionBreachedAtUtc { get; private set; }

    private IncidentSla() { }

    public IncidentSla(
        Guid incidentId,
        IncidentPriority priority,
        int responseTargetMinutes,
        int resolutionTargetMinutes,
        DateTimeOffset createdAtUtc)
    {
        if (incidentId == Guid.Empty)
            throw new ArgumentException("Incident ID is required.", nameof(incidentId));

        if (!Enum.IsDefined(priority))
            throw new ArgumentOutOfRangeException(nameof(priority));

        if (responseTargetMinutes <= 0 || resolutionTargetMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(responseTargetMinutes));

        Id = Guid.NewGuid();
        IncidentId = incidentId;
        Priority = priority;
        ResponseTargetMinutes = responseTargetMinutes;
        ResolutionTargetMinutes = resolutionTargetMinutes;
        ResponseDueAtUtc = createdAtUtc.AddMinutes(responseTargetMinutes);
        ResolutionDueAtUtc = createdAtUtc.AddMinutes(resolutionTargetMinutes);
    }

    public void RecordFirstResponse(DateTimeOffset occurredAtUtc)
    {
        if (FirstResponseAtUtc is not null)
            return;

        FirstResponseAtUtc = occurredAtUtc;

        if (occurredAtUtc > ResponseDueAtUtc)
            ResponseBreachedAtUtc ??= ResponseDueAtUtc;
    }

    public void RecordResolution(DateTimeOffset occurredAtUtc)
    {
        ResolvedAtUtc = occurredAtUtc;

        if (occurredAtUtc > ResolutionDueAtUtc)
            ResolutionBreachedAtUtc ??= occurredAtUtc;
    }

    public void ReopenResolutionClock()
    {
        ResolvedAtUtc = null;
    }

    public void MarkResponseBreached(DateTimeOffset detectedAtUtc)
    {
        if (FirstResponseAtUtc is null &&
            detectedAtUtc >= ResponseDueAtUtc)
        {
            ResponseBreachedAtUtc ??= detectedAtUtc;
        }
    }

    public void MarkResolutionBreached(DateTimeOffset detectedAtUtc)
    {
        if (ResolvedAtUtc is null &&
            detectedAtUtc >= ResolutionDueAtUtc)
        {
            ResolutionBreachedAtUtc ??= detectedAtUtc;
        }
    }
}
