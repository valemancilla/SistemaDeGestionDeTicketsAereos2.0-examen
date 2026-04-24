using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Infrastructure.Repositories;

public sealed class AirportTimeZoneRepository : IAirportTimeZoneRepository
{
    private readonly AppDbContext _dbContext;

    public AirportTimeZoneRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AirportTimeZone?> GetByIdAsync(int idAirport, int idTimeZone, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AirportTimeZoneEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdAirport == idAirport && x.IdTimeZone == idTimeZone, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<AirportTimeZone>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<AirportTimeZoneEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdAirport).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<AirportTimeZone>> ListByAirportAsync(int idAirport, CancellationToken ct = default)
    {
        var query = _dbContext.Set<AirportTimeZoneEntity>().AsNoTracking();
        query = query.Where(x => x.IdAirport == idAirport);
        var entities = await query.OrderBy(x => x.IdAirport).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(AirportTimeZone airportTimeZone, CancellationToken ct = default)
    {
        var entity = ToEntity(airportTimeZone);
        await _dbContext.Set<AirportTimeZoneEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(AirportTimeZone airportTimeZone, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AirportTimeZoneEntity>().FirstOrDefaultAsync(x => x.IdAirport == airportTimeZone.IdAirport && x.IdTimeZone == airportTimeZone.IdTimeZone, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("AirportTimeZone was not found.");
        }

        // IdAirport e IdTimeZone conforman la clave compuesta — no hay campos adicionales que actualizar
    }

    public async Task RemoveAsync(int idAirport, int idTimeZone, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AirportTimeZoneEntity>().FirstOrDefaultAsync(x => x.IdAirport == idAirport && x.IdTimeZone == idTimeZone, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<AirportTimeZoneEntity>().Remove(entity);
    }

    private static AirportTimeZone ToDomain(AirportTimeZoneEntity entity)
    {
        return AirportTimeZone.Create(entity.IdAirport, entity.IdTimeZone);
    }

    private static AirportTimeZoneEntity ToEntity(AirportTimeZone aggregate)
    {
        return new AirportTimeZoneEntity
        {
            IdAirport = aggregate.IdAirport,
            IdTimeZone = aggregate.IdTimeZone
        };
    }
}
