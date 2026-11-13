using OpsPilot.Domain.Enums;

namespace OpsPilot.Domain.Entities;

public sealed class SlaPolicy
{
    public Guid Id { get; private set; }
    public IncidentPriority Priority { get; private set; }
    public int ResponseTargetMinutes { get; private set; }
    public int ResolutionTargetMinutes { get; private set; }
    public bool IsActive { get; private set; }

    private SlaPolicy() { }

    public SlaPolicy(
        IncidentPriority priority,
        int responseTargetMinutes,
        int resolutionTargetMinutes)
    {
        if (!Enum.IsDefined(priority))
            throw new ArgumentOutOfRangeException(nameof(priority));

        ValidateTargets(responseTargetMinutes, resolutionTargetMinutes);

        Id = Guid.NewGuid();
        Priority = priority;
        ResponseTargetMinutes = responseTargetMinutes;
        ResolutionTargetMinutes = resolutionTargetMinutes;
        IsActive = true;
    }

    public void UpdateTargets(int responseTargetMinutes, int resolutionTargetMinutes)
    {
        ValidateTargets(responseTargetMinutes, resolutionTargetMinutes);
        ResponseTargetMinutes = responseTargetMinutes;
        ResolutionTargetMinutes = resolutionTargetMinutes;
    }

    public void SetActive(bool isActive) => IsActive = isActive;

    private static void ValidateTargets(int response, int resolution)
    {
        if (response <= 0 || resolution <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(response),
                "SLA targets must be greater than zero.");
    }
}
