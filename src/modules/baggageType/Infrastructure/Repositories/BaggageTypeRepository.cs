using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Infrastructure.Repositories;

public sealed class BaggageTypeRepository : IBaggageTypeRepository
{
    private readonly AppDbContext _dbContext;

    public BaggageTypeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BaggageType?> GetByIdAsync(BaggageTypeId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BaggageTypeEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdBaggageType == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<BaggageType>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<BaggageTypeEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdBaggageType).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(BaggageType baggageType, CancellationToken ct = default)
    {
        var entity = ToEntity(baggageType);
        await _dbContext.Set<BaggageTypeEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(BaggageType baggageType, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BaggageTypeEntity>().FirstOrDefaultAsync(x => x.IdBaggageType == baggageType.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("BaggageType was not found.");
        }

        var values = ToEntity(baggageType);
        entity.TypeName = values.TypeName;
    }

    public async Task DeleteAsync(BaggageTypeId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BaggageTypeEntity>().FirstOrDefaultAsync(x => x.IdBaggageType == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<BaggageTypeEntity>().Remove(entity);
    }

    private static BaggageType ToDomain(BaggageTypeEntity entity)
    {
        return BaggageType.Create(entity.IdBaggageType, entity.TypeName);
    }

    private static BaggageTypeEntity ToEntity(BaggageType aggregate)
    {
        return new BaggageTypeEntity
        {
            IdBaggageType = aggregate.Id.Value,
            TypeName = aggregate.Name.Value
        };
    }
}
