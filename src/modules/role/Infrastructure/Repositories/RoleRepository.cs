using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Infrastructure.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _dbContext;

    public RoleRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role?> GetByIdAsync(RoleId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<RoleEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdUserRole == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Role>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<RoleEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdUserRole).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Role role, CancellationToken ct = default)
    {
        var entity = ToEntity(role);
        await _dbContext.Set<RoleEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Role role, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<RoleEntity>().FirstOrDefaultAsync(x => x.IdUserRole == role.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Role was not found.");
        }

        var values = ToEntity(role);
        entity.RoleName = values.RoleName;
    }

    public async Task DeleteAsync(RoleId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<RoleEntity>().FirstOrDefaultAsync(x => x.IdUserRole == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<RoleEntity>().Remove(entity);
    }

    private static Role ToDomain(RoleEntity entity)
    {
        return Role.Create(entity.IdUserRole, entity.RoleName);
    }

    private static RoleEntity ToEntity(Role aggregate)
    {
        return new RoleEntity
        {
            IdUserRole = aggregate.Id.Value,
            RoleName = aggregate.Name.Value
        };
    }
}
