using Microsoft.EntityFrameworkCore;
using OpsPilot.Application.Interfaces;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Infrastructure.Persistence.Repositories;

public sealed class MaintenanceRepository : IMaintenanceRepository
{
    private readonly OpsPilotDbContext _context;

    public MaintenanceRepository(OpsPilotDbContext context)
    {
        _context = context;
    }

    public Task<List<MaintenanceRecord>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.MaintenanceRecords
            .AsNoTracking()
            .Include(x => x.Asset)
            .OrderBy(x => x.ScheduledAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<MaintenanceRecord?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.MaintenanceRecords
            .Include(x => x.Asset)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        MaintenanceRecord record,
        CancellationToken cancellationToken = default)
    {
        await _context.MaintenanceRecords.AddAsync(record, cancellationToken);
    }
}
