using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Infrastructure.Repositories;

public sealed class GenderRepository : IGenderRepository
{
    private readonly AppDbContext _dbContext;

    public GenderRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Gender?> GetByIdAsync(GenderId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<GenderEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdGender == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Gender>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<GenderEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdGender).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Gender gender, CancellationToken ct = default)
    {
        var entity = ToEntity(gender);
        await _dbContext.Set<GenderEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Gender gender, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<GenderEntity>().FirstOrDefaultAsync(x => x.IdGender == gender.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Gender was not found.");
        }

        var values = ToEntity(gender);
        entity.Description = values.Description;
    }

    public async Task DeleteAsync(GenderId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<GenderEntity>().FirstOrDefaultAsync(x => x.IdGender == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<GenderEntity>().Remove(entity);
    }

    private static Gender ToDomain(GenderEntity entity)
    {
        return Gender.Create(entity.IdGender, entity.Description);
    }

    private static GenderEntity ToEntity(Gender aggregate)
    {
        return new GenderEntity
        {
            IdGender = aggregate.Id.Value,
            Description = aggregate.Description.Value
        };
    }
}
