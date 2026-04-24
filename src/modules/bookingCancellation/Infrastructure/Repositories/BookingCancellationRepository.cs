using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Infrastructure.Repositories;

public sealed class BookingCancellationRepository : IBookingCancellationRepository
{
    private readonly AppDbContext _dbContext;

    public BookingCancellationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingCancellation?> GetByIdAsync(BookingCancellationId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingCancellationEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdCancellation == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<BookingCancellation>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<BookingCancellationEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdCancellation).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<BookingCancellation>> ListByBookingAsync(int idBooking, CancellationToken ct = default)
    {
        var query = _dbContext.Set<BookingCancellationEntity>().AsNoTracking();
        query = query.Where(x => x.IdBooking == idBooking);
        var entities = await query.OrderBy(x => x.IdCancellation).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(BookingCancellation cancellation, CancellationToken ct = default)
    {
        var entity = ToEntity(cancellation);
        await _dbContext.Set<BookingCancellationEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(BookingCancellation cancellation, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingCancellationEntity>().FirstOrDefaultAsync(x => x.IdBooking == cancellation.IdBooking && x.IdUser == cancellation.IdUser, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("BookingCancellation was not found.");
        }

        var values = ToEntity(cancellation);
        entity.CancellationReason = values.CancellationReason;
        entity.PenaltyAmount = values.PenaltyAmount;
        entity.CancellationDate = values.CancellationDate;
    }

    public async Task DeleteAsync(BookingCancellationId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingCancellationEntity>().FirstOrDefaultAsync(x => x.IdCancellation == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<BookingCancellationEntity>().Remove(entity);
    }

    private static BookingCancellation ToDomain(BookingCancellationEntity entity)
    {
        return BookingCancellation.Create(entity.IdCancellation, entity.CancellationDate, entity.CancellationReason, entity.PenaltyAmount, entity.IdBooking, entity.IdUser);
    }

    private static BookingCancellationEntity ToEntity(BookingCancellation aggregate)
    {
        return new BookingCancellationEntity
        {
            IdCancellation = aggregate.Id.Value,
            IdBooking = aggregate.IdBooking,
            CancellationReason = aggregate.Reason.Value,
            PenaltyAmount = aggregate.PenaltyAmount.Value,
            CancellationDate = aggregate.CancellationDate.Value,
            IdUser = aggregate.IdUser
        };
    }
}
