using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Repositories;

public sealed class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _dbContext;

    public TicketRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Ticket?> GetByIdAsync(TicketId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<TicketEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdTicket == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Ticket?> GetByCodeAsync(string ticketCode, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<TicketEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TicketCode == ticketCode, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Ticket>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<TicketEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdTicket).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Ticket ticket, CancellationToken ct = default)
    {
        var entity = ToEntity(ticket);
        await _dbContext.Set<TicketEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Ticket ticket, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<TicketEntity>()
            .FirstOrDefaultAsync(x => x.IdTicket == ticket.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Ticket was not found.");
        }

        var values = ToEntity(ticket);
        entity.TicketCode = values.TicketCode;
        entity.IssueDate = values.IssueDate;
        entity.IdBooking = values.IdBooking;
        entity.IdFare = values.IdFare;
        entity.IdStatus = values.IdStatus;
    }

    public async Task DeleteAsync(TicketId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<TicketEntity>().FirstOrDefaultAsync(x => x.IdTicket == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<TicketEntity>().Remove(entity);
    }

    private static Ticket ToDomain(TicketEntity entity)
    {
        return Ticket.Create(entity.IdTicket, entity.TicketCode, entity.IssueDate, entity.IdBooking, entity.IdFare, entity.IdStatus);
    }

    private static TicketEntity ToEntity(Ticket aggregate)
    {
        return new TicketEntity
        {
            IdTicket = aggregate.Id.Value,
            TicketCode = aggregate.Code.Value,
            IdBooking = aggregate.IdBooking,
            IdFare = aggregate.IdFare,
            IdStatus = aggregate.IdStatus,
            IssueDate = aggregate.IssueDate.Value
        };
    }
}
