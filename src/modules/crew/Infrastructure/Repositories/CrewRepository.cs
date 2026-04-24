using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Infrastructure.Repositories;

public sealed class CrewRepository : ICrewRepository
{
    private readonly AppDbContext _dbContext;

    public CrewRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Crew?> GetByIdAsync(CrewId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CrewEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdCrew == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Crew>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<CrewEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdCrew).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Crew crew, CancellationToken ct = default)
    {
        var entity = ToEntity(crew);
        await _dbContext.Set<CrewEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Crew crew, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CrewEntity>().FirstOrDefaultAsync(x => x.IdCrew == crew.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Crew was not found.");
        }

        var values = ToEntity(crew);
        entity.GroupName = values.GroupName;
    }

    public async Task DeleteAsync(CrewId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CrewEntity>().FirstOrDefaultAsync(x => x.IdCrew == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<CrewEntity>().Remove(entity);
    }

    private static Crew ToDomain(CrewEntity entity)
    {
        return Crew.Create(entity.IdCrew, entity.GroupName);
    }

    private static CrewEntity ToEntity(Crew aggregate)
    {
        return new CrewEntity
        {
            IdCrew = aggregate.Id.Value,
            GroupName = aggregate.GroupName.Value
        };
    }
}
