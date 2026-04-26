using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Repositories;

public sealed class BookingCustomerRepository : IBookingCustomerRepository
{
    private readonly AppDbContext _dbContext;

    public BookingCustomerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingCustomer?> GetByIdAsync(BookingCustomerId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingCustomerEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdBookingCustomer == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<BookingCustomer>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<BookingCustomerEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdBookingCustomer).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<BookingCustomer>> ListByBookingAsync(int idBooking, CancellationToken ct = default)
    {
        var query = _dbContext.Set<BookingCustomerEntity>().AsNoTracking();
        query = query.Where(x => x.IdBooking == idBooking);
        var entities = await query.OrderBy(x => x.IdBookingCustomer).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(BookingCustomer bookingCustomer, CancellationToken ct = default)
    {
        var entity = ToEntity(bookingCustomer);
        await _dbContext.Set<BookingCustomerEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(BookingCustomer bookingCustomer, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingCustomerEntity>().FirstOrDefaultAsync(x => x.IdBookingCustomer == bookingCustomer.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("BookingCustomer was not found.");
        }

        var values = ToEntity(bookingCustomer);
        entity.IdBooking = values.IdBooking;
        entity.IdUser = values.IdUser;
        entity.IdPerson = values.IdPerson;
        entity.IdSeat = values.IdSeat;
        entity.IsPrimary = values.IsPrimary;
        entity.AssociationDate = values.AssociationDate;
    }

    public async Task DeleteAsync(BookingCustomerId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BookingCustomerEntity>().FirstOrDefaultAsync(x => x.IdBookingCustomer == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<BookingCustomerEntity>().Remove(entity);
    }

    private static BookingCustomer ToDomain(BookingCustomerEntity entity)
    {
        return BookingCustomer.Create(
            entity.IdBookingCustomer,
            entity.AssociationDate,
            entity.IdBooking,
            entity.IdUser,
            entity.IdPerson,
            entity.IdSeat,
            entity.IsPrimary,
            entity.IsReadyToBoard);
    }

    private static BookingCustomerEntity ToEntity(BookingCustomer aggregate)
    {
        return new BookingCustomerEntity
        {
            IdBookingCustomer = aggregate.Id.Value,
            IdBooking = aggregate.IdBooking,
            IdUser = aggregate.IdUser,
            IdPerson = aggregate.IdPerson,
            IdSeat = aggregate.IdSeat,
            IsPrimary = aggregate.IsPrimary,
            IsReadyToBoard = aggregate.IsReadyToBoard,
            AssociationDate = aggregate.AssociationDate.Value
        };
    }
}
