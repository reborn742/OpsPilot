using Microsoft.EntityFrameworkCore;
using OpsPilot.Application.Interfaces;
using OpsPilot.Domain.Entities;
using OpsPilot.Domain.Enums;
using OpsPilot.Infrastructure.Persistence;

namespace OpsPilot.Infrastructure.Persistence.Repositories;

public sealed class SlaRepository : ISlaRepository
{
    private readonly OpsPilotDbContext _context;

    public SlaRepository(OpsPilotDbContext context)
    {
        _context = context;
    }

    public Task<SlaPolicy?> GetActivePolicyAsync(
        IncidentPriority priority,
        CancellationToken cancellationToken = default)
    {
        return _context.SlaPolicies.FirstOrDefaultAsync(
            x => x.Priority == priority && x.IsActive,
            cancellationToken);
    }

    public Task<IncidentSla?> GetByIncidentIdAsync(
        Guid incidentId,
        CancellationToken cancellationToken = default)
    {
        return _context.IncidentSlas.FirstOrDefaultAsync(
            x => x.IncidentId == incidentId,
            cancellationToken);
    }

    public async Task AddIncidentSlaAsync(
        IncidentSla incidentSla,
        CancellationToken cancellationToken = default)
    {
        await _context.IncidentSlas.AddAsync(
            incidentSla, cancellationToken);
    }
}
