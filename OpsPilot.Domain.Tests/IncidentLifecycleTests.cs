using Xunit;
using OpsPilot.Domain.Entities;
using OpsPilot.Domain.Enums;

namespace OpsPilot.Domain.Tests;

public class IncidentLifecycleTests
{
    private static Incident NewIncident() => new(
        "Laptop fails to boot",
        "Startup never completes.",
        IncidentCategory.Hardware,
        IncidentPriority.High);

    private static Incident InProgressIncident()
    {
        var incident = NewIncident();
        incident.Assign(Guid.NewGuid());
        incident.StartWork();
        return incident;
    }

    [Fact]
    public void New_incident_starts_open()
    {
        var incident = NewIncident();

        Assert.Equal(IncidentStatus.Open, incident.Status);
        Assert.NotEqual(Guid.Empty, incident.Id);
        Assert.Null(incident.ResolvedAtUtc);
    }

    [Theory]
    [InlineData("", "description")]
    [InlineData("title", "  ")]
    public void Constructor_rejects_blank_title_or_description(string title, string description)
    {
        Assert.Throws<ArgumentException>(() => new Incident(
            title, description, IncidentCategory.Software, IncidentPriority.Low));
    }

    [Fact]
    public void Assigning_an_open_incident_moves_it_to_assigned()
    {
        var incident = NewIncident();
        var technicianId = Guid.NewGuid();

        incident.Assign(technicianId);

        Assert.Equal(IncidentStatus.Assigned, incident.Status);
        Assert.Equal(technicianId, incident.AssignedTechnicianId);
    }

    [Fact]
    public void Cannot_start_work_before_assignment()
    {
        var incident = NewIncident();

        Assert.Throws<InvalidOperationException>(incident.StartWork);
    }

    [Fact]
    public void Resolving_requires_notes()
    {
        var incident = InProgressIncident();

        Assert.Throws<ArgumentException>(() => incident.Resolve("   "));
    }

    [Fact]
    public void Full_lifecycle_reaches_closed()
    {
        var incident = InProgressIncident();

        incident.PutOnHold();
        Assert.Equal(IncidentStatus.Pending, incident.Status);

        incident.ResumeWork();
        incident.Resolve("Replaced faulty RAM.");
        Assert.Equal(IncidentStatus.Resolved, incident.Status);
        Assert.NotNull(incident.ResolvedAtUtc);

        incident.Close();
        Assert.Equal(IncidentStatus.Closed, incident.Status);
        Assert.NotNull(incident.ClosedAtUtc);
    }

    [Fact]
    public void Cannot_close_an_unresolved_incident()
    {
        var incident = InProgressIncident();

        Assert.Throws<InvalidOperationException>(incident.Close);
    }

    [Fact]
    public void Reopening_clears_resolution_data()
    {
        var incident = InProgressIncident();
        incident.Resolve("Fixed.");

        incident.Reopen();

        Assert.Equal(IncidentStatus.InProgress, incident.Status);
        Assert.Null(incident.ResolutionNotes);
        Assert.Null(incident.ResolvedAtUtc);
    }

    [Fact]
    public void Cannot_reopen_an_active_incident()
    {
        var incident = NewIncident();

        Assert.Throws<InvalidOperationException>(incident.Reopen);
    }
}
