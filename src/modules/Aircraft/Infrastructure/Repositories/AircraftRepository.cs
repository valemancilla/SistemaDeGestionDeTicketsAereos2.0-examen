using Microsoft.EntityFrameworkCore;
using AircraftClass = global::SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.aggregate.Aircraft;
using AircraftAggregate = SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.aggregate.Aircraft;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Repositories;

public sealed class AircraftRepository : IAircraftRepository
{
    private readonly AppDbContext _dbContext;

    public AircraftRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AircraftClass?> GetByIdAsync(AircraftId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AircraftEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdAircraft == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<AircraftClass>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<AircraftEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdAircraft).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<AircraftClass>> ListByAirlineAsync(int idAirline, CancellationToken ct = default)
    {
        var query = _dbContext.Set<AircraftEntity>().AsNoTracking();
        query = query.Where(x => x.IdAirline == idAirline);
        var entities = await query.OrderBy(x => x.IdAircraft).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(AircraftClass aircraft, CancellationToken ct = default)
    {
        var entity = ToEntity(aircraft);
        await _dbContext.Set<AircraftEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(AircraftClass aircraft, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AircraftEntity>()
            .FirstOrDefaultAsync(x => x.IdAircraft == aircraft.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Aircraft was not found.");
        }

        var values = ToEntity(aircraft);
        entity.IdAirline = values.IdAirline;
        entity.IdModel = values.IdModel;
        entity.Capacity = values.Capacity;
    }

    public async Task DeleteAsync(AircraftId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AircraftEntity>().FirstOrDefaultAsync(x => x.IdAircraft == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        var aircraftId = id.Value;

        // 1) Eliminar vuelos operados por esta aeronave (y toda su “rama” de dependencias)
        var flightIds = await _dbContext.Set<FlightEntity>()
            .AsNoTracking()
            .Where(f => f.IdAircraft == aircraftId)
            .Select(f => f.IdFlight)
            .ToListAsync(ct);

        foreach (var flightId in flightIds)
        {
            // Asientos generados para el vuelo
            var seatFlightsByFlight = await _dbContext.Set<SeatFlightEntity>()
                .Where(sf => sf.IdFlight == flightId)
                .ToListAsync(ct);
            if (seatFlightsByFlight.Count > 0)
                _dbContext.Set<SeatFlightEntity>().RemoveRange(seatFlightsByFlight);

            // Historial de estados del vuelo
            var flightHist = await _dbContext.Set<FlightStatusHistoryEntity>()
                .Where(h => h.IdFlight == flightId)
                .ToListAsync(ct);
            if (flightHist.Count > 0)
                _dbContext.Set<FlightStatusHistoryEntity>().RemoveRange(flightHist);

            // Reservas del vuelo
            var bookingIds = await _dbContext.Set<BookingEntity>()
                .AsNoTracking()
                .Where(b => b.IdFlight == flightId)
                .Select(b => b.IdBooking)
                .ToListAsync(ct);

            if (bookingIds.Count > 0)
            {
                var payments = await _dbContext.Set<PaymentEntity>()
                    .Where(p => bookingIds.Contains(p.IdBooking))
                    .ToListAsync(ct);
                if (payments.Count > 0)
                    _dbContext.Set<PaymentEntity>().RemoveRange(payments);

                var ticketIds = await _dbContext.Set<TicketEntity>()
                    .AsNoTracking()
                    .Where(t => bookingIds.Contains(t.IdBooking))
                    .Select(t => t.IdTicket)
                    .ToListAsync(ct);

                if (ticketIds.Count > 0)
                {
                    var baggages = await _dbContext.Set<BaggageEntity>()
                        .Where(b => ticketIds.Contains(b.IdTicket))
                        .ToListAsync(ct);
                    if (baggages.Count > 0)
                        _dbContext.Set<BaggageEntity>().RemoveRange(baggages);

                    var checkIns = await _dbContext.Set<CheckInEntity>()
                        .Where(c => ticketIds.Contains(c.IdTicket))
                        .ToListAsync(ct);
                    if (checkIns.Count > 0)
                        _dbContext.Set<CheckInEntity>().RemoveRange(checkIns);

                    var ticketHist = await _dbContext.Set<TicketStatusHistoryEntity>()
                        .Where(h => ticketIds.Contains(h.IdTicket))
                        .ToListAsync(ct);
                    if (ticketHist.Count > 0)
                        _dbContext.Set<TicketStatusHistoryEntity>().RemoveRange(ticketHist);
                }

                var tickets = await _dbContext.Set<TicketEntity>()
                    .Where(t => ticketIds.Contains(t.IdTicket))
                    .ToListAsync(ct);
                if (tickets.Count > 0)
                    _dbContext.Set<TicketEntity>().RemoveRange(tickets);

                var bookingCustomers = await _dbContext.Set<BookingCustomerEntity>()
                    .Where(bc => bookingIds.Contains(bc.IdBooking))
                    .ToListAsync(ct);
                if (bookingCustomers.Count > 0)
                    _dbContext.Set<BookingCustomerEntity>().RemoveRange(bookingCustomers);

                var bookingStatusHist = await _dbContext.Set<BookingStatusHistoryEntity>()
                    .Where(h => bookingIds.Contains(h.IdBooking))
                    .ToListAsync(ct);
                if (bookingStatusHist.Count > 0)
                    _dbContext.Set<BookingStatusHistoryEntity>().RemoveRange(bookingStatusHist);

                var cancellations = await _dbContext.Set<BookingCancellationEntity>()
                    .Where(c => bookingIds.Contains(c.IdBooking))
                    .ToListAsync(ct);
                if (cancellations.Count > 0)
                    _dbContext.Set<BookingCancellationEntity>().RemoveRange(cancellations);

                var bookings = await _dbContext.Set<BookingEntity>()
                    .Where(b => bookingIds.Contains(b.IdBooking))
                    .ToListAsync(ct);
                if (bookings.Count > 0)
                    _dbContext.Set<BookingEntity>().RemoveRange(bookings);
            }

            var flightEntity = await _dbContext.Set<FlightEntity>()
                .FirstOrDefaultAsync(f => f.IdFlight == flightId, ct);
            if (flightEntity is not null)
                _dbContext.Set<FlightEntity>().Remove(flightEntity);
        }

        // 2) Eliminar asientos físicos del avión y sus dependencias directas (si quedaran)
        var seatIds = await _dbContext.Set<SeatEntity>()
            .AsNoTracking()
            .Where(s => s.IdAircraft == aircraftId)
            .Select(s => s.IdSeat)
            .ToListAsync(ct);

        if (seatIds.Count > 0)
        {
            var seatFlightsBySeat = await _dbContext.Set<SeatFlightEntity>()
                .Where(sf => seatIds.Contains(sf.IdSeat))
                .ToListAsync(ct);
            if (seatFlightsBySeat.Count > 0)
                _dbContext.Set<SeatFlightEntity>().RemoveRange(seatFlightsBySeat);

            var bookingCustomersBySeat = await _dbContext.Set<BookingCustomerEntity>()
                .Where(bc => seatIds.Contains(bc.IdSeat))
                .ToListAsync(ct);
            if (bookingCustomersBySeat.Count > 0)
                _dbContext.Set<BookingCustomerEntity>().RemoveRange(bookingCustomersBySeat);

            var checkInsBySeat = await _dbContext.Set<CheckInEntity>()
                .Where(ci => seatIds.Contains(ci.IdSeat))
                .ToListAsync(ct);
            if (checkInsBySeat.Count > 0)
                _dbContext.Set<CheckInEntity>().RemoveRange(checkInsBySeat);

            var seats = await _dbContext.Set<SeatEntity>()
                .Where(s => s.IdAircraft == aircraftId)
                .ToListAsync(ct);
            if (seats.Count > 0)
                _dbContext.Set<SeatEntity>().RemoveRange(seats);
        }

        _dbContext.Set<AircraftEntity>().Remove(entity);
    }

    private static AircraftAggregate ToDomain(AircraftEntity entity)
    {
        return AircraftAggregate.Create(entity.IdAircraft, entity.Capacity, entity.IdAirline, entity.IdModel);
    }

    private static AircraftEntity ToEntity(AircraftAggregate aggregate)
    {
        return new AircraftEntity
        {
            IdAircraft = aggregate.Id.Value,
            IdAirline = aggregate.IdAirline,
            IdModel = aggregate.IdModel,
            Capacity = aggregate.Capacity.Value
        };
    }
}
