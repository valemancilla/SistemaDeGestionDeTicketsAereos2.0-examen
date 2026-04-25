using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Repositories;

public sealed class FlightRepository : IFlightRepository
{
    private readonly AppDbContext _dbContext;

    public FlightRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Flight?> GetByIdAsync(FlightId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<FlightEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdFlight == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Flight?> GetByFlightNumberAsync(string flightNumber, DateOnly date, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<FlightEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FlightNumber == flightNumber && x.Date == date, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Flight>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<FlightEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdFlight).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Flight flight, CancellationToken ct = default)
    {
        var entity = ToEntity(flight);
        await _dbContext.Set<FlightEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Flight flight, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<FlightEntity>()
            .FirstOrDefaultAsync(x => x.IdFlight == flight.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Flight was not found.");
        }

        var values = ToEntity(flight);
        entity.FlightNumber = values.FlightNumber;
        entity.Date = values.Date;
        entity.DepartureTime = values.DepartureTime;
        entity.ArrivalTime = values.ArrivalTime;
        entity.TotalCapacity = values.TotalCapacity;
        entity.AvailableSeats = values.AvailableSeats;
        entity.IdRoute = values.IdRoute;
        entity.IdAircraft = values.IdAircraft;
        entity.IdStatus = values.IdStatus;
        entity.IdCrew = values.IdCrew;
        entity.IdFare = values.IdFare;
        entity.BoardingGate = values.BoardingGate;
    }

    public async Task DeleteAsync(FlightId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<FlightEntity>().FirstOrDefaultAsync(x => x.IdFlight == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        // Eliminar dependencias (FK Restrict) antes del vuelo para permitir borrado “total”.
        var flightId = id.Value;

        // 1) Asientos generados para el vuelo
        var seatFlights = await _dbContext.Set<SeatFlightEntity>()
            .Where(sf => sf.IdFlight == flightId)
            .ToListAsync(ct);
        if (seatFlights.Count > 0)
            _dbContext.Set<SeatFlightEntity>().RemoveRange(seatFlights);

        // 2) Historial de estados del vuelo
        var flightHistory = await _dbContext.Set<FlightStatusHistoryEntity>()
            .Where(h => h.IdFlight == flightId)
            .ToListAsync(ct);
        if (flightHistory.Count > 0)
            _dbContext.Set<FlightStatusHistoryEntity>().RemoveRange(flightHistory);

        // 3) Reservas del vuelo y todas sus dependencias (tickets, pagos, clientes, historiales, cancelaciones)
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

            // 3.a) Hijos del ticket (Restrict): Baggage, CheckIn, TicketStatusHistory
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

            var bookingHist = await _dbContext.Set<BookingStatusHistoryEntity>()
                .Where(h => bookingIds.Contains(h.IdBooking))
                .ToListAsync(ct);
            if (bookingHist.Count > 0)
                _dbContext.Set<BookingStatusHistoryEntity>().RemoveRange(bookingHist);

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

        _dbContext.Set<FlightEntity>().Remove(entity);
    }

    private static Flight ToDomain(FlightEntity entity)
    {
        return Flight.Create(
            entity.IdFlight,
            entity.FlightNumber,
            entity.Date,
            entity.DepartureTime,
            entity.ArrivalTime,
            entity.TotalCapacity,
            entity.AvailableSeats,
            entity.IdRoute,
            entity.IdAircraft,
            entity.IdStatus,
            entity.IdCrew,
            entity.IdFare,
            entity.BoardingGate);
    }

    private static FlightEntity ToEntity(Flight aggregate)
    {
        return new FlightEntity
        {
            IdFlight = aggregate.Id.Value,
            IdRoute = aggregate.IdRoute,
            IdAircraft = aggregate.IdAircraft,
            FlightNumber = aggregate.Number.Value,
            Date = aggregate.Date.Value,
            DepartureTime = aggregate.DepartureTime.Value,
            ArrivalTime = aggregate.ArrivalTime.Value,
            TotalCapacity = aggregate.TotalCapacity.Value,
            AvailableSeats = aggregate.AvailableSeats.Value,
            IdStatus = aggregate.IdStatus,
            IdCrew = aggregate.IdCrew,
            IdFare = aggregate.IdFare,
            BoardingGate = aggregate.BoardingGate
        };
    }
}
