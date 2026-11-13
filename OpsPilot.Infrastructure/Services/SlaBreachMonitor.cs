using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpsPilot.Infrastructure.Persistence;

namespace OpsPilot.Infrastructure.Services;

public sealed class SlaBreachMonitor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SlaBreachMonitor> _logger;
    private readonly TimeSpan _interval;

    public SlaBreachMonitor(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<SlaBreachMonitor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var seconds = configuration.GetValue<int?>(
            "SlaMonitoring:IntervalSeconds") ?? 60;

        _interval = TimeSpan.FromSeconds(Math.Max(10, seconds));
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckBreachesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SLA breach monitoring failed.");
            }

            try
            {
                if (!await timer.WaitForNextTickAsync(stoppingToken))
                    break;
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task CheckBreachesAsync(
        CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var db = scope.ServiceProvider
            .GetRequiredService<OpsPilotDbContext>();

        var now = DateTimeOffset.UtcNow;

        var overdueSlas = await db.IncidentSlas
            .Where(x =>
                (x.ResponseBreachedAtUtc == null &&
                 x.FirstResponseAtUtc == null &&
                 x.ResponseDueAtUtc <= now)
                ||
                (x.ResolutionBreachedAtUtc == null &&
                 x.ResolvedAtUtc == null &&
                 x.ResolutionDueAtUtc <= now))
            .ToListAsync(cancellationToken);

        var updatedCount = 0;

        foreach (var sla in overdueSlas)
        {
            var responseOverdue =
                sla.ResponseBreachedAtUtc == null &&
                sla.FirstResponseAtUtc == null &&
                sla.ResponseDueAtUtc <= now;

            var resolutionOverdue =
                sla.ResolutionBreachedAtUtc == null &&
                sla.ResolvedAtUtc == null &&
                sla.ResolutionDueAtUtc <= now;

            sla.MarkResponseBreached(now);
            sla.MarkResolutionBreached(now);

            if (responseOverdue || resolutionOverdue)
                updatedCount++;
        }

        if (updatedCount > 0)
        {
            await db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "SLA monitoring updated {Count} overdue SLA records.",
                updatedCount);
        }
    }
}
