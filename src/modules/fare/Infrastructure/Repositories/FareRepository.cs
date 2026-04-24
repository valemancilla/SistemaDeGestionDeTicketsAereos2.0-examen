using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Repositories;

public sealed class FareRepository : IFareRepository
{
    private readonly AppDbContext _dbContext;

    public FareRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fare?> GetByIdAsync(FareId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<FareEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdFare == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Fare>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<FareEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdFare).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Fare fare, CancellationToken ct = default)
    {
        var entity = ToEntity(fare);
        await _dbContext.Set<FareEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Fare fare, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<FareEntity>()
            .FirstOrDefaultAsync(x => x.IdFare == fare.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Fare was not found.");
        }

        var values = ToEntity(fare);
        entity.FareName = values.FareName;
        entity.BasePrice = values.BasePrice;
        entity.ValidFrom = values.ValidFrom;
        entity.ValidTo = values.ValidTo;
        entity.Active = values.Active;
        entity.ExpirationDate = values.ExpirationDate;
    }

    public async Task DeleteAsync(FareId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<FareEntity>().FirstOrDefaultAsync(x => x.IdFare == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<FareEntity>().Remove(entity);
    }

    private static Fare ToDomain(FareEntity entity)
    {
        return Fare.Create(entity.IdFare, entity.FareName, entity.BasePrice, entity.ValidFrom, entity.ValidTo, entity.ExpirationDate, entity.IdAirline, entity.Active);
    }

    private static FareEntity ToEntity(Fare aggregate)
    {
        return new FareEntity
        {
            IdFare = aggregate.Id.Value,
            FareName = aggregate.Name.Value,
            BasePrice = aggregate.BasePrice.Value,
            IdAirline = aggregate.IdAirline,
            ValidFrom = aggregate.ValidFrom.Value,
            ValidTo = aggregate.ValidTo.Value,
            Active = aggregate.Active,
            ExpirationDate = aggregate.ExpirationDate.Value
        };
    }
}
