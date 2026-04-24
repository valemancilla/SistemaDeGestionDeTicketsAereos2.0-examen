using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Infrastructure.Repositories;

public sealed class PersonAddressRepository : IPersonAddressRepository
{
    private readonly AppDbContext _dbContext;

    public PersonAddressRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PersonAddress?> GetByIdAsync(PersonAddressId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PersonAddressEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdAddress == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<PersonAddress>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<PersonAddressEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdAddress).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<PersonAddress>> ListByPersonAsync(int idPerson, CancellationToken ct = default)
    {
        var query = _dbContext.Set<PersonAddressEntity>().AsNoTracking();
        query = query.Where(x => x.IdPerson == idPerson);
        var entities = await query.OrderBy(x => x.IdAddress).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(PersonAddress address, CancellationToken ct = default)
    {
        var entity = ToEntity(address);
        await _dbContext.Set<PersonAddressEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(PersonAddress address, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PersonAddressEntity>().FirstOrDefaultAsync(x => x.IdAddress == address.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("PersonAddress was not found.");
        }

        var values = ToEntity(address);
        entity.IdPerson = values.IdPerson;
        entity.IdCity = values.IdCity;
        entity.Street = values.Street;
        entity.Number = values.Number;
        entity.Neighborhood = values.Neighborhood;
        entity.DwellingType = values.DwellingType;
        entity.ZipCode = values.ZipCode;
        entity.Active = values.Active;
    }

    public async Task DeleteAsync(PersonAddressId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PersonAddressEntity>().FirstOrDefaultAsync(x => x.IdAddress == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<PersonAddressEntity>().Remove(entity);
    }

    private static PersonAddress ToDomain(PersonAddressEntity entity)
    {
        return PersonAddress.Create(
            entity.IdAddress,
            entity.Street,
            entity.Number,
            entity.Neighborhood,
            entity.DwellingType,
            entity.ZipCode,
            entity.IdPerson,
            entity.IdCity,
            entity.Active);
    }

    private static PersonAddressEntity ToEntity(PersonAddress aggregate)
    {
        return new PersonAddressEntity
        {
            IdAddress = aggregate.Id.Value,
            IdPerson = aggregate.IdPerson,
            Street = aggregate.Street.Value,
            Number = aggregate.Number.Value,
            Neighborhood = aggregate.Neighborhood.Value,
            DwellingType = aggregate.DwellingType.Value,
            IdCity = aggregate.IdCity,
            ZipCode = aggregate.ZipCode.Value,
            Active = aggregate.Active
        };
    }
}
