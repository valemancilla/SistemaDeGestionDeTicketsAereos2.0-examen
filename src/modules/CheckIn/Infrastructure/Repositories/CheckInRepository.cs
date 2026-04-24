using Microsoft.EntityFrameworkCore;
using CheckInClass = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;
using CheckInAggregate = SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Repositories;

public sealed class CheckInRepository : ICheckInRepository
{
    private readonly AppDbContext _dbContext;

    public CheckInRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CheckInClass?> GetByIdAsync(CheckInId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CheckInEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdCheckIn == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<CheckInClass>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<CheckInEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdCheckIn).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(CheckInClass checkIn, CancellationToken ct = default)
    {
        var entity = ToEntity(checkIn);
        await _dbContext.Set<CheckInEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(CheckInClass checkIn, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CheckInEntity>().FirstOrDefaultAsync(x => x.IdTicket == checkIn.IdTicket && x.IdChannel == checkIn.IdChannel && x.IdSeat == checkIn.IdSeat && x.IdUser == checkIn.IdUser && x.IdStatus == checkIn.IdStatus, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("CheckIn was not found.");
        }

        var values = ToEntity(checkIn);
        entity.CheckInDate = values.CheckInDate;
    }

    public async Task DeleteAsync(CheckInId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CheckInEntity>().FirstOrDefaultAsync(x => x.IdCheckIn == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<CheckInEntity>().Remove(entity);
    }

    private static CheckInAggregate ToDomain(CheckInEntity entity)
    {
        return CheckInAggregate.Create(entity.IdCheckIn, entity.CheckInDate, entity.IdTicket, entity.IdChannel, entity.IdSeat, entity.IdUser, entity.IdStatus);
    }

    private static CheckInEntity ToEntity(CheckInAggregate aggregate)
    {
        return new CheckInEntity
        {
            IdCheckIn = aggregate.Id.Value,
            IdTicket = aggregate.IdTicket,
            CheckInDate = aggregate.Date.Value,
            IdChannel = aggregate.IdChannel,
            IdSeat = aggregate.IdSeat,
            IdUser = aggregate.IdUser,
            IdStatus = aggregate.IdStatus
        };
    }
}
