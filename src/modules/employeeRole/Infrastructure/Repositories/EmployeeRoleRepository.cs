using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Infrastructure.Repositories;

public sealed class EmployeeRoleRepository : IEmployeeRoleRepository
{
    private readonly AppDbContext _dbContext;

    public EmployeeRoleRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EmployeeRole?> GetByIdAsync(EmployeeRoleId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<EmployeeRoleEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdRole == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<EmployeeRole>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<EmployeeRoleEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdRole).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(EmployeeRole employeeRole, CancellationToken ct = default)
    {
        var entity = ToEntity(employeeRole);
        await _dbContext.Set<EmployeeRoleEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(EmployeeRole employeeRole, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<EmployeeRoleEntity>().FirstOrDefaultAsync(x => x.IdRole == employeeRole.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("EmployeeRole was not found.");
        }

        var values = ToEntity(employeeRole);
        entity.RoleName = values.RoleName;
    }

    public async Task DeleteAsync(EmployeeRoleId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<EmployeeRoleEntity>().FirstOrDefaultAsync(x => x.IdRole == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<EmployeeRoleEntity>().Remove(entity);
    }

    private static EmployeeRole ToDomain(EmployeeRoleEntity entity)
    {
        return EmployeeRole.Create(entity.IdRole, entity.RoleName);
    }

    private static EmployeeRoleEntity ToEntity(EmployeeRole aggregate)
    {
        return new EmployeeRoleEntity
        {
            IdRole = aggregate.Id.Value,
            RoleName = aggregate.Name.Value
        };
    }
}
