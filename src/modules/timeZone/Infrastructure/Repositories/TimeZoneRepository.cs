using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Infrastructure.Repositories;

public sealed class TimeZoneRepository : ITimeZoneRepository
{
    private readonly AppDbContext _dbContext;

    public TimeZoneRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AirlineTimeZone?> GetByIdAsync(TimeZoneId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<TimeZoneEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdTimeZone == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<AirlineTimeZone>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<TimeZoneEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdTimeZone).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(AirlineTimeZone timeZone, CancellationToken ct = default)
    {
        var entity = ToEntity(timeZone);
        await _dbContext.Set<TimeZoneEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(AirlineTimeZone timeZone, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<TimeZoneEntity>().FirstOrDefaultAsync(x => x.IdTimeZone == timeZone.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("AirlineTimeZone was not found.");
        }

        var values = ToEntity(timeZone);
        entity.Name = values.Name;
        entity.UTCOffset = values.UTCOffset;
    }

    public async Task DeleteAsync(TimeZoneId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<TimeZoneEntity>().FirstOrDefaultAsync(x => x.IdTimeZone == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<TimeZoneEntity>().Remove(entity);
    }

    private static AirlineTimeZone ToDomain(TimeZoneEntity entity)
    {
        return AirlineTimeZone.Create(entity.IdTimeZone, entity.Name, entity.UTCOffset);
    }

    private static TimeZoneEntity ToEntity(AirlineTimeZone aggregate)
    {
        return new TimeZoneEntity
        {
            IdTimeZone = aggregate.Id.Value,
            Name = aggregate.Name.Value,
            UTCOffset = aggregate.UTCOffset.Value
        };
    }
}
