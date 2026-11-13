using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpsPilot.Domain.Entities;
using OpsPilot.Domain.Enums;
using OpsPilot.Infrastructure.Persistence;

namespace OpsPilot.Infrastructure.Services;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<OpsPilotDbContext>();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var assets = new[]
        {
            CreateAsset("DEMO-LAP-001", "Developer Laptop", AssetCategory.Laptop, "Dell", "Latitude 5450", "DEMO-SN-001", today.AddYears(-1), AssetStatus.Active),
            CreateAsset("DEMO-LAP-002", "HR Laptop", AssetCategory.Laptop, "HP", "ProBook 450", "DEMO-SN-002", today.AddMonths(-8), AssetStatus.Active),
            CreateAsset("DEMO-DES-001", "Finance Desktop", AssetCategory.Desktop, "Lenovo", "ThinkCentre M70", "DEMO-SN-003", today.AddYears(-2), AssetStatus.Active),
            CreateAsset("DEMO-SRV-001", "Application Server", AssetCategory.Server, "Dell", "PowerEdge R550", "DEMO-SN-004", today.AddYears(-1), AssetStatus.Active),
            CreateAsset("DEMO-NET-001", "Core Network Switch", AssetCategory.NetworkDevice, "Cisco", "Catalyst 9200", "DEMO-SN-005", today.AddMonths(-6), AssetStatus.Active),
            CreateAsset("DEMO-PRN-001", "Accounts Printer", AssetCategory.Printer, "HP", "LaserJet Pro", "DEMO-SN-006", today.AddYears(-2), AssetStatus.InRepair),
            CreateAsset("DEMO-MOB-001", "Support Smartphone", AssetCategory.MobileDevice, "Samsung", "Galaxy A55", "DEMO-SN-007", today.AddMonths(-4), AssetStatus.Active),
            CreateAsset("DEMO-LAP-003", "Training Room Laptop", AssetCategory.Laptop, "Lenovo", "ThinkPad E14", "DEMO-SN-008", today.AddYears(-3), AssetStatus.Active),
            CreateAsset("DEMO-DES-002", "Old Reception Desktop", AssetCategory.Desktop, "HP", "EliteDesk 800", "DEMO-SN-009", today.AddYears(-5), AssetStatus.Retired),
            CreateAsset("DEMO-PER-001", "Damaged Keyboard", AssetCategory.Peripheral, "Logitech", "K120", "DEMO-SN-010", today.AddYears(-2), AssetStatus.Disposed)
        };

        foreach (var asset in assets)
        {
            var exists = await db.Assets.AnyAsync(x => x.AssetTag == asset.AssetTag);

            if (!exists)
                await db.Assets.AddAsync(asset);
        }

        await db.SaveChangesAsync();

        var assetIds = await db.Assets
            .Where(x => x.AssetTag.StartsWith("DEMO-"))
            .ToDictionaryAsync(x => x.AssetTag, x => x.Id);

        var incidents = new[]
        {
            CreateIncident("DEMO-INC-001", "Laptop fails to boot", "Developer laptop does not complete the startup process.", IncidentCategory.Hardware, IncidentPriority.Critical, assetIds["DEMO-LAP-001"]),
            CreateIncident("DEMO-INC-002", "Application server unavailable", "Internal application server is not responding.", IncidentCategory.Network, IncidentPriority.Critical, assetIds["DEMO-SRV-001"]),
            CreateIncident("DEMO-INC-003", "Core switch connectivity issue", "Intermittent connectivity reported across office network.", IncidentCategory.Network, IncidentPriority.Critical, assetIds["DEMO-NET-001"]),
            CreateIncident("DEMO-INC-004", "HR software login failure", "HR user cannot access the internal HR application.", IncidentCategory.Access, IncidentPriority.High, assetIds["DEMO-LAP-002"]),
            CreateIncident("DEMO-INC-005", "Desktop performance degradation", "Finance desktop is running significantly slower than usual.", IncidentCategory.Hardware, IncidentPriority.High, assetIds["DEMO-DES-001"]),
            CreateIncident("DEMO-INC-006", "Printer paper feed problem", "Accounts printer repeatedly reports a paper feed error.", IncidentCategory.Hardware, IncidentPriority.High, assetIds["DEMO-PRN-001"]),
            CreateIncident("DEMO-INC-007", "Email client configuration", "Email client requires configuration after an update.", IncidentCategory.Software, IncidentPriority.Medium, assetIds["DEMO-LAP-002"]),
            CreateIncident("DEMO-INC-008", "Wireless connection instability", "Training room laptop intermittently loses Wi-Fi connectivity.", IncidentCategory.Network, IncidentPriority.Medium, assetIds["DEMO-LAP-003"]),
            CreateIncident("DEMO-INC-009", "Software installation request", "User requires an approved development tool installation.", IncidentCategory.Software, IncidentPriority.Medium, assetIds["DEMO-LAP-001"]),
            CreateIncident("DEMO-INC-010", "Mobile device synchronization", "Support smartphone is not synchronizing company contacts.", IncidentCategory.Software, IncidentPriority.Low, assetIds["DEMO-MOB-001"]),
            CreateIncident("DEMO-INC-011", "Keyboard replacement request", "Reception requires a replacement keyboard.", IncidentCategory.Hardware, IncidentPriority.Low, assetIds["DEMO-PER-001"]),
            CreateIncident("DEMO-INC-012", "Account access request", "Employee requires access to an internal shared resource.", IncidentCategory.Access, IncidentPriority.Low, null)
        };

        foreach (var incident in incidents)
        {
            var exists = await db.Incidents.AnyAsync(x => x.Title == incident.Title);

            if (!exists)
                await db.Incidents.AddAsync(incident);
        }

        await db.SaveChangesAsync();

        // Seed default SLA policies without overwriting existing settings.
        var defaultPolicies = new[]
        {
            (Priority: IncidentPriority.Low, Response: 480, Resolution: 2880),
            (Priority: IncidentPriority.Medium, Response: 240, Resolution: 1440),
            (Priority: IncidentPriority.High, Response: 60, Resolution: 480),
            (Priority: IncidentPriority.Critical, Response: 15, Resolution: 120)
        };

        foreach (var item in defaultPolicies)
        {
            if (!await db.SlaPolicies.AnyAsync(x => x.Priority == item.Priority))
            {
                await db.SlaPolicies.AddAsync(
                    new SlaPolicy(item.Priority, item.Response, item.Resolution));
            }
        }

        await db.SaveChangesAsync();

        // Backfill all incidents that do not yet have an SLA snapshot.
        var incidentsWithoutSla = await db.Incidents
            .Where(incident =>
                !db.IncidentSlas.Any(sla => sla.IncidentId == incident.Id))
            .ToListAsync();

        foreach (var incident in incidentsWithoutSla)
        {
            var policy = await db.SlaPolicies.FirstAsync(
                x => x.Priority == incident.Priority && x.IsActive);

            var sla = new IncidentSla(
                incident.Id,
                incident.Priority,
                policy.ResponseTargetMinutes,
                policy.ResolutionTargetMinutes,
                incident.CreatedAtUtc);

            if (incident.ResolvedAtUtc.HasValue)
                sla.RecordResolution(incident.ResolvedAtUtc.Value);

            await db.IncidentSlas.AddAsync(sla);
        }

        await db.SaveChangesAsync();
    }

    private static Asset CreateAsset(
        string tag,
        string name,
        AssetCategory category,
        string manufacturer,
        string model,
        string serial,
        DateOnly purchaseDate,
        AssetStatus status)
    {
        var asset = new Asset(
            tag, name, category, manufacturer, model, serial,
            purchaseDate, purchaseDate.AddYears(3));

        switch (status)
        {
            case AssetStatus.InRepair:
                asset.MarkForRepair();
                asset.UpdateCondition(AssetCondition.Poor);
                break;

            case AssetStatus.Retired:
                asset.Retire();
                break;

            case AssetStatus.Disposed:
                asset.Dispose();
                asset.UpdateCondition(AssetCondition.Damaged);
                break;
        }

        return asset;
    }

    private static Incident CreateIncident(
        string code,
        string title,
        string description,
        IncidentCategory category,
        IncidentPriority priority,
        Guid? assetId)
    {
        return new Incident(
            $"[DEMO] {code} - {title}",
            description,
            category,
            priority,
            assetId);
    }
}
