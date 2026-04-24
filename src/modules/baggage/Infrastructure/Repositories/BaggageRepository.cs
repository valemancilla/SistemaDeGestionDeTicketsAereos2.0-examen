using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Repositories;

public sealed class BaggageRepository : IBaggageRepository
{
    private readonly AppDbContext _dbContext;

    public BaggageRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Baggage?> GetByIdAsync(BaggageId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BaggageEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdBaggage == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Baggage>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<BaggageEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdBaggage).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Baggage>> ListByTicketAsync(int idTicket, CancellationToken ct = default)
    {
        var query = _dbContext.Set<BaggageEntity>().AsNoTracking();
        query = query.Where(x => x.IdTicket == idTicket);
        var entities = await query.OrderBy(x => x.IdBaggage).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Baggage baggage, CancellationToken ct = default)
    {
        var entity = ToEntity(baggage);
        await _dbContext.Set<BaggageEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Baggage baggage, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BaggageEntity>().FirstOrDefaultAsync(x => x.IdTicket == baggage.IdTicket && x.IdBaggageType == baggage.IdBaggageType, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Baggage was not found.");
        }

        var values = ToEntity(baggage);
        entity.Weight = values.Weight;
    }

    public async Task DeleteAsync(BaggageId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BaggageEntity>().FirstOrDefaultAsync(x => x.IdBaggage == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<BaggageEntity>().Remove(entity);
    }

    private static Baggage ToDomain(BaggageEntity entity)
    {
        return Baggage.Create(entity.IdBaggage, entity.Weight, entity.IdTicket, entity.IdBaggageType);
    }

    private static BaggageEntity ToEntity(Baggage aggregate)
    {
        return new BaggageEntity
        {
            IdBaggage = aggregate.Id.Value,
            IdTicket = aggregate.IdTicket,
            IdBaggageType = aggregate.IdBaggageType,
            Weight = aggregate.Weight.Value
        };
    }
}
