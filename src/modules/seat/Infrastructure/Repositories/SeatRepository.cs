using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Repositories;

public sealed class SeatRepository : ISeatRepository
{
    private readonly AppDbContext _dbContext;

    public SeatRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Seat?> GetByIdAsync(SeatId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SeatEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdSeat == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Seat>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<SeatEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdSeat).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Seat>> ListByAircraftAsync(int idAircraft, CancellationToken ct = default)
    {
        var query = _dbContext.Set<SeatEntity>().AsNoTracking();
        query = query.Where(x => x.IdAircraft == idAircraft);
        var entities = await query.OrderBy(x => x.IdSeat).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Seat seat, CancellationToken ct = default)
    {
        var entity = ToEntity(seat);
        await _dbContext.Set<SeatEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Seat seat, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SeatEntity>().FirstOrDefaultAsync(x => x.IdAircraft == seat.IdAircraft && x.IdClase == seat.IdClase, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Seat was not found.");
        }

        var values = ToEntity(seat);
        entity.Number = values.Number;
    }

    public async Task DeleteAsync(SeatId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SeatEntity>().FirstOrDefaultAsync(x => x.IdSeat == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<SeatEntity>().Remove(entity);
    }

    private static Seat ToDomain(SeatEntity entity)
    {
        return Seat.Create(entity.IdSeat, entity.Number, entity.IdAircraft, entity.IdClase);
    }

    private static SeatEntity ToEntity(Seat aggregate)
    {
        return new SeatEntity
        {
            IdSeat = aggregate.Id.Value,
            IdAircraft = aggregate.IdAircraft,
            Number = aggregate.Number.Value,
            IdClase = aggregate.IdClase
        };
    }
}
