using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Repositories;

public sealed class SeatClassRepository : ISeatClassRepository
{
    private readonly AppDbContext _dbContext;

    public SeatClassRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SeatClass?> GetByIdAsync(SeatClassId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SeatClassEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdClase == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<SeatClass>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<SeatClassEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdClase).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(SeatClass seatClass, CancellationToken ct = default)
    {
        var entity = ToEntity(seatClass);
        await _dbContext.Set<SeatClassEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(SeatClass seatClass, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SeatClassEntity>().FirstOrDefaultAsync(x => x.IdClase == seatClass.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("SeatClass was not found.");
        }

        var values = ToEntity(seatClass);
        entity.ClassName = values.ClassName;
    }

    public async Task DeleteAsync(SeatClassId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SeatClassEntity>().FirstOrDefaultAsync(x => x.IdClase == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        // Si hay asientos que usan esta clase, hay que limpiar sus dependencias primero (FK RESTRICT).
        var seatIds = await _dbContext.Set<SeatEntity>()
            .AsNoTracking()
            .Where(s => s.IdClase == id.Value)
            .Select(s => s.IdSeat)
            .ToListAsync(ct);

        if (seatIds.Count > 0)
        {
            var seatFlights = await _dbContext.Set<SeatFlightEntity>()
                .Where(sf => seatIds.Contains(sf.IdSeat))
                .ToListAsync(ct);
            if (seatFlights.Count > 0) _dbContext.Set<SeatFlightEntity>().RemoveRange(seatFlights);

            var bookingCustomers = await _dbContext.Set<BookingCustomerEntity>()
                .Where(bc => seatIds.Contains(bc.IdSeat))
                .ToListAsync(ct);
            if (bookingCustomers.Count > 0) _dbContext.Set<BookingCustomerEntity>().RemoveRange(bookingCustomers);

            var checkIns = await _dbContext.Set<CheckInEntity>()
                .Where(ci => seatIds.Contains(ci.IdSeat))
                .ToListAsync(ct);
            if (checkIns.Count > 0) _dbContext.Set<CheckInEntity>().RemoveRange(checkIns);

            var seats = await _dbContext.Set<SeatEntity>()
                .Where(s => s.IdClase == id.Value)
                .ToListAsync(ct);
            if (seats.Count > 0) _dbContext.Set<SeatEntity>().RemoveRange(seats);
        }

        // También puede existir precio por clase en una tarifa (FK RESTRICT).
        var fareSeatClassPrices = await _dbContext.Set<FareSeatClassPriceEntity>()
            .Where(x => x.IdClase == id.Value)
            .ToListAsync(ct);
        if (fareSeatClassPrices.Count > 0)
            _dbContext.Set<FareSeatClassPriceEntity>().RemoveRange(fareSeatClassPrices);

        _dbContext.Set<SeatClassEntity>().Remove(entity);
    }

    private static SeatClass ToDomain(SeatClassEntity entity)
    {
        return SeatClass.Create(entity.IdClase, entity.ClassName);
    }

    private static SeatClassEntity ToEntity(SeatClass aggregate)
    {
        return new SeatClassEntity
        {
            IdClase = aggregate.Id.Value,
            ClassName = aggregate.Name.Value
        };
    }
}
