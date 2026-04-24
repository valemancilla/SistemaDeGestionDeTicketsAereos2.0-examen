using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Infrastructure.Repositories;

public sealed class CityRepository : ICityRepository
{
    private readonly AppDbContext _dbContext;

    public CityRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<City?> GetByIdAsync(CityId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CityEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdCity == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<City>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<CityEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdCity).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<City>> ListByCountryAsync(int idCountry, CancellationToken ct = default)
    {
        var query = _dbContext.Set<CityEntity>().AsNoTracking();
        query = query.Where(x => x.IdCountry == idCountry);
        var entities = await query.OrderBy(x => x.IdCity).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(City city, CancellationToken ct = default)
    {
        var entity = ToEntity(city);
        await _dbContext.Set<CityEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(City city, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CityEntity>().FirstOrDefaultAsync(x => x.IdCountry == city.IdCountry, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("City was not found.");
        }

        var values = ToEntity(city);
        entity.Name = values.Name;
    }

    public async Task DeleteAsync(CityId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CityEntity>().FirstOrDefaultAsync(x => x.IdCity == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<CityEntity>().Remove(entity);
    }

    private static City ToDomain(CityEntity entity)
    {
        return City.Create(entity.IdCity, entity.Name, entity.IdCountry);
    }

    private static CityEntity ToEntity(City aggregate)
    {
        return new CityEntity
        {
            IdCity = aggregate.Id.Value,
            Name = aggregate.Name.Value,
            IdCountry = aggregate.IdCountry
        };
    }
}
