using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Repositories;

public sealed class TicketStatusHistoryRepository : ITicketStatusHistoryRepository
{
    private readonly AppDbContext _dbContext;

    public TicketStatusHistoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TicketStatusHistory?> GetByIdAsync(TicketStatusHistoryId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<TicketStatusHistoryEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdHistory == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<TicketStatusHistory>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<TicketStatusHistoryEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdHistory).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<TicketStatusHistory>> ListByTicketAsync(int idTicket, CancellationToken ct = default)
    {
        var query = _dbContext.Set<TicketStatusHistoryEntity>().AsNoTracking();
        query = query.Where(x => x.IdTicket == idTicket);
        var entities = await query.OrderBy(x => x.IdHistory).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(TicketStatusHistory history, CancellationToken ct = default)
    {
        var entity = ToEntity(history);
        await _dbContext.Set<TicketStatusHistoryEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(TicketStatusHistory history, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<TicketStatusHistoryEntity>().FirstOrDefaultAsync(x => x.IdTicket == history.IdTicket && x.IdStatus == history.IdStatus && x.IdUser == history.IdUser, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("TicketStatusHistory was not found.");
        }

        var values = ToEntity(history);
        entity.ChangeDate = values.ChangeDate;
        entity.Observation = values.Observation;
    }

    public async Task DeleteAsync(TicketStatusHistoryId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<TicketStatusHistoryEntity>().FirstOrDefaultAsync(x => x.IdHistory == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<TicketStatusHistoryEntity>().Remove(entity);
    }

    private static TicketStatusHistory ToDomain(TicketStatusHistoryEntity entity)
    {
        return TicketStatusHistory.Create(entity.IdHistory, entity.ChangeDate, entity.Observation, entity.IdTicket, entity.IdStatus, entity.IdUser);
    }

    private static TicketStatusHistoryEntity ToEntity(TicketStatusHistory aggregate)
    {
        return new TicketStatusHistoryEntity
        {
            IdHistory = aggregate.Id.Value,
            IdTicket = aggregate.IdTicket,
            IdStatus = aggregate.IdStatus,
            ChangeDate = aggregate.ChangeDate.Value,
            IdUser = aggregate.IdUser,
            Observation = aggregate.Observation.Value
        };
    }
}
