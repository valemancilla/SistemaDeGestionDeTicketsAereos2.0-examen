using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Repositories;

public sealed class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _dbContext;

    public BookingRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Booking?> GetByIdAsync(BookingId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdBooking == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Booking?> GetByCodeAsync(string bookingCode, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BookingCode == bookingCode, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Booking>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<BookingEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdBooking).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Booking booking, CancellationToken ct = default)
    {
        var entity = ToEntity(booking);
        await _dbContext.Set<BookingEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Booking booking, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingEntity>()
            .FirstOrDefaultAsync(x => x.IdBooking == booking.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Booking was not found.");
        }

        // El agregado de dominio no incluye titular/consentimientos: no pisar columnas persistidas al actualizar.
        var holderEmail = entity.HolderEmail;
        var holderPhonePrefix = entity.HolderPhonePrefix;
        var holderPhone = entity.HolderPhone;
        var consentDataProcessing = entity.ConsentDataProcessing;
        var consentMarketing = entity.ConsentMarketing;
        var idHolderPerson = entity.IdHolderPerson;

        var values = ToEntity(booking);
        entity.BookingCode = values.BookingCode;
        entity.FlightDate = values.FlightDate;
        entity.IdFlight = values.IdFlight;
        entity.IdStatus = values.IdStatus;
        entity.SeatCount = values.SeatCount;
        entity.CreationDate = values.CreationDate;
        entity.Observations = values.Observations;

        entity.HolderEmail = holderEmail;
        entity.HolderPhonePrefix = holderPhonePrefix;
        entity.HolderPhone = holderPhone;
        entity.ConsentDataProcessing = consentDataProcessing;
        entity.ConsentMarketing = consentMarketing;
        entity.IdHolderPerson = idHolderPerson;
    }

    public async Task DeleteAsync(BookingId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingEntity>().FirstOrDefaultAsync(x => x.IdBooking == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<BookingEntity>().Remove(entity);
    }

    private static Booking ToDomain(BookingEntity entity)
    {
        return Booking.Create(entity.IdBooking, entity.BookingCode, entity.FlightDate, entity.CreationDate, entity.SeatCount, entity.Observations, entity.IdFlight, entity.IdStatus);
    }

    private static BookingEntity ToEntity(Booking aggregate)
    {
        return new BookingEntity
        {
            IdBooking = aggregate.Id.Value,
            BookingCode = aggregate.Code.Value,
            FlightDate = aggregate.FlightDate.Value,
            IdStatus = aggregate.IdStatus,
            IdFlight = aggregate.IdFlight,
            SeatCount = aggregate.SeatCount.Value,
            CreationDate = aggregate.CreationDate.Value,
            Observations = aggregate.Observations.Value
        };
    }
}
