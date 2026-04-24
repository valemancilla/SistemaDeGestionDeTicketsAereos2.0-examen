using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Infrastructure.Repositories;

public sealed class CrewMemberRepository : ICrewMemberRepository
{
    private readonly AppDbContext _dbContext;

    public CrewMemberRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CrewMember?> GetByIdAsync(CrewMemberId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CrewMemberEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdCrewMember == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<CrewMember>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<CrewMemberEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdCrewMember).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<CrewMember>> ListByCrewAsync(int idCrew, CancellationToken ct = default)
    {
        var query = _dbContext.Set<CrewMemberEntity>().AsNoTracking();
        query = query.Where(x => x.IdCrew == idCrew);
        var entities = await query.OrderBy(x => x.IdCrewMember).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(CrewMember crewMember, CancellationToken ct = default)
    {
        var entity = ToEntity(crewMember);
        await _dbContext.Set<CrewMemberEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(CrewMember crewMember, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CrewMemberEntity>().FirstOrDefaultAsync(x => x.IdCrewMember == crewMember.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException($"CrewMember with id '{crewMember.Id.Value}' was not found.");
        }

        entity.IdCrew = crewMember.IdCrew;
        entity.IdEmployee = crewMember.IdEmployee;
        entity.IdRole = crewMember.IdRole;
    }

    public async Task DeleteAsync(CrewMemberId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CrewMemberEntity>().FirstOrDefaultAsync(x => x.IdCrewMember == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<CrewMemberEntity>().Remove(entity);
    }

    private static CrewMember ToDomain(CrewMemberEntity entity)
    {
        return CrewMember.Create(entity.IdCrewMember, entity.IdCrew, entity.IdEmployee, entity.IdRole);
    }

    private static CrewMemberEntity ToEntity(CrewMember aggregate)
    {
        return new CrewMemberEntity
        {
            IdCrewMember = aggregate.Id.Value,
            IdCrew = aggregate.IdCrew,
            IdEmployee = aggregate.IdEmployee,
            IdRole = aggregate.IdRole
        };
    }
}
