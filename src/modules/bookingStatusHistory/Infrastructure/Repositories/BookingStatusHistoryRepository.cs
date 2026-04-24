using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Infrastructure.Repositories;

public sealed class BookingStatusHistoryRepository : IBookingStatusHistoryRepository
{
    private readonly AppDbContext _dbContext;

    public BookingStatusHistoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingStatusHistory?> GetByIdAsync(BookingStatusHistoryId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingStatusHistoryEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdHistory == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<BookingStatusHistory>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<BookingStatusHistoryEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdHistory).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<BookingStatusHistory>> ListByBookingAsync(int idBooking, CancellationToken ct = default)
    {
        var query = _dbContext.Set<BookingStatusHistoryEntity>().AsNoTracking();
        query = query.Where(x => x.IdBooking == idBooking);
        var entities = await query.OrderBy(x => x.IdHistory).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(BookingStatusHistory history, CancellationToken ct = default)
    {
        var entity = ToEntity(history);
        await _dbContext.Set<BookingStatusHistoryEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(BookingStatusHistory history, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingStatusHistoryEntity>().FirstOrDefaultAsync(x => x.IdBooking == history.IdBooking && x.IdStatus == history.IdStatus && x.IdUser == history.IdUser, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("BookingStatusHistory was not found.");
        }

        var values = ToEntity(history);
        entity.ChangeDate = values.ChangeDate;
        entity.Observation = values.Observation;
    }

    public async Task DeleteAsync(BookingStatusHistoryId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingStatusHistoryEntity>().FirstOrDefaultAsync(x => x.IdHistory == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<BookingStatusHistoryEntity>().Remove(entity);
    }

    private static BookingStatusHistory ToDomain(BookingStatusHistoryEntity entity)
    {
        return BookingStatusHistory.Create(entity.IdHistory, entity.ChangeDate, entity.Observation, entity.IdBooking, entity.IdStatus, entity.IdUser);
    }

    private static BookingStatusHistoryEntity ToEntity(BookingStatusHistory aggregate)
    {
        return new BookingStatusHistoryEntity
        {
            IdHistory = aggregate.Id.Value,
            IdBooking = aggregate.IdBooking,
            IdStatus = aggregate.IdStatus,
            ChangeDate = aggregate.ChangeDate.Value,
            IdUser = aggregate.IdUser,
            Observation = aggregate.Observation.Value
        };
    }
}
