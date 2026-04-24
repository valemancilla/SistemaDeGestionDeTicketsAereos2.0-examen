using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Infrastructure.Repositories;

public sealed class FlightStatusHistoryRepository : IFlightStatusHistoryRepository
{
    private readonly AppDbContext _dbContext;

    public FlightStatusHistoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FlightStatusHistory?> GetByIdAsync(FlightStatusHistoryId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<FlightStatusHistoryEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdHistory == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<FlightStatusHistory>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<FlightStatusHistoryEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdHistory).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<FlightStatusHistory>> ListByFlightAsync(int idFlight, CancellationToken ct = default)
    {
        var query = _dbContext.Set<FlightStatusHistoryEntity>().AsNoTracking();
        query = query.Where(x => x.IdFlight == idFlight);
        var entities = await query.OrderBy(x => x.IdHistory).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(FlightStatusHistory history, CancellationToken ct = default)
    {
        var entity = ToEntity(history);
        await _dbContext.Set<FlightStatusHistoryEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(FlightStatusHistory history, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<FlightStatusHistoryEntity>().FirstOrDefaultAsync(x => x.IdFlight == history.IdFlight && x.IdStatus == history.IdStatus && x.IdUser == history.IdUser, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("FlightStatusHistory was not found.");
        }

        var values = ToEntity(history);
        entity.ChangeDate = values.ChangeDate;
        entity.Observation = values.Observation;
    }

    public async Task DeleteAsync(FlightStatusHistoryId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<FlightStatusHistoryEntity>().FirstOrDefaultAsync(x => x.IdHistory == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<FlightStatusHistoryEntity>().Remove(entity);
    }

    private static FlightStatusHistory ToDomain(FlightStatusHistoryEntity entity)
    {
        return FlightStatusHistory.Create(entity.IdHistory, entity.ChangeDate, entity.Observation, entity.IdFlight, entity.IdStatus, entity.IdUser);
    }

    private static FlightStatusHistoryEntity ToEntity(FlightStatusHistory aggregate)
    {
        return new FlightStatusHistoryEntity
        {
            IdHistory = aggregate.Id.Value,
            IdFlight = aggregate.IdFlight,
            IdStatus = aggregate.IdStatus,
            ChangeDate = aggregate.ChangeDate.Value,
            IdUser = aggregate.IdUser,
            Observation = aggregate.Observation.Value
        };
    }
}
