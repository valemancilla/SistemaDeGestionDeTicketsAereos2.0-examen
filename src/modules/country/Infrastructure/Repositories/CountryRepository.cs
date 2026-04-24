using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Repositories;

public sealed class CountryRepository : ICountryRepository
{
    private readonly AppDbContext _dbContext;

    public CountryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Country?> GetByIdAsync(CountryId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CountryEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdCountry == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Country?> GetByIsoCodeAsync(string isoCode, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CountryEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ISOCode == isoCode, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Country>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<CountryEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdCountry).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Country country, CancellationToken ct = default)
    {
        var entity = ToEntity(country);
        await _dbContext.Set<CountryEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Country country, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CountryEntity>().FirstOrDefaultAsync(x => x.IdCountry == country.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Country was not found.");
        }

        var values = ToEntity(country);
        entity.Name = values.Name;
        entity.ISOCode = values.ISOCode;
    }

    public async Task DeleteAsync(CountryId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CountryEntity>().FirstOrDefaultAsync(x => x.IdCountry == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<CountryEntity>().Remove(entity);
    }

    private static Country ToDomain(CountryEntity entity)
    {
        return Country.Create(entity.IdCountry, entity.Name, entity.ISOCode);
    }

    private static CountryEntity ToEntity(Country aggregate)
    {
        return new CountryEntity
        {
            IdCountry = aggregate.Id.Value,
            Name = aggregate.Name.Value,
            ISOCode = aggregate.ISOCode.Value
        };
    }
}
