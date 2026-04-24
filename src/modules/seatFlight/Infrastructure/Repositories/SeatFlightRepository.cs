using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Repositories;

public sealed class SeatFlightRepository : ISeatFlightRepository
{
    private readonly AppDbContext _dbContext;

    public SeatFlightRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SeatFlight?> GetByIdAsync(SeatFlightId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SeatFlightEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdSeatFlight == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<SeatFlight?> GetBySeatAndFlightAsync(int idSeat, int idFlight, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SeatFlightEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdSeat == idSeat && x.IdFlight == idFlight, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<SeatFlight>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<SeatFlightEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdSeatFlight).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(SeatFlight seatFlight, CancellationToken ct = default)
    {
        var entity = ToEntity(seatFlight);
        await _dbContext.Set<SeatFlightEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(SeatFlight seatFlight, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SeatFlightEntity>().FirstOrDefaultAsync(x => x.IdSeat == seatFlight.IdSeat && x.IdFlight == seatFlight.IdFlight, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("SeatFlight was not found.");
        }

        var values = ToEntity(seatFlight);
        entity.Available = values.Available;
    }

    public async Task DeleteAsync(SeatFlightId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SeatFlightEntity>().FirstOrDefaultAsync(x => x.IdSeatFlight == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<SeatFlightEntity>().Remove(entity);
    }

    private static SeatFlight ToDomain(SeatFlightEntity entity)
    {
        return SeatFlight.Create(entity.IdSeatFlight, entity.IdSeat, entity.IdFlight, entity.Available);
    }

    private static SeatFlightEntity ToEntity(SeatFlight aggregate)
    {
        return new SeatFlightEntity
        {
            IdSeatFlight = aggregate.Id.Value,
            IdSeat = aggregate.IdSeat,
            IdFlight = aggregate.IdFlight,
            Available = aggregate.Available
        };
    }
}
