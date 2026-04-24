using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Repositories;

public sealed class SystemStatusRepository : ISystemStatusRepository
{
    private readonly AppDbContext _dbContext;

    public SystemStatusRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SystemStatus?> GetByIdAsync(SystemStatusId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SystemStatusEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdStatus == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<SystemStatus>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<SystemStatusEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdStatus).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<SystemStatus>> ListByEntityTypeAsync(string entityType, CancellationToken ct = default)
    {
        var query = _dbContext.Set<SystemStatusEntity>().AsNoTracking();
        query = query.Where(x => x.EntityType == entityType);
        var entities = await query.OrderBy(x => x.IdStatus).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(SystemStatus systemStatus, CancellationToken ct = default)
    {
        var entity = ToEntity(systemStatus);
        await _dbContext.Set<SystemStatusEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(SystemStatus systemStatus, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SystemStatusEntity>().FirstOrDefaultAsync(x => x.IdStatus == systemStatus.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("SystemStatus was not found.");
        }

        var values = ToEntity(systemStatus);
        entity.StatusName = values.StatusName;
        entity.EntityType = values.EntityType;
    }

    public async Task DeleteAsync(SystemStatusId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<SystemStatusEntity>().FirstOrDefaultAsync(x => x.IdStatus == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<SystemStatusEntity>().Remove(entity);
    }

    private static SystemStatus ToDomain(SystemStatusEntity entity)
    {
        return SystemStatus.Create(entity.IdStatus, entity.StatusName, entity.EntityType);
    }

    private static SystemStatusEntity ToEntity(SystemStatus aggregate)
    {
        return new SystemStatusEntity
        {
            IdStatus = aggregate.Id.Value,
            StatusName = aggregate.Name.Value,
            EntityType = aggregate.EntityType.Value
        };
    }
}
