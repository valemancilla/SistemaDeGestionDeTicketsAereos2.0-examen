using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Repositories;

public sealed class AirportRepository : IAirportRepository
{
    private readonly AppDbContext _dbContext;

    public AirportRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Airport?> GetByIdAsync(AirportId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AirportEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdAirport == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Airport?> GetByIataCodeAsync(string iataCode, CancellationToken ct = default)
    {
        var normalizedIatacode = iataCode.Trim().ToUpperInvariant();
        var entity = await _dbContext.Set<AirportEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IATACode == normalizedIatacode, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Airport>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<AirportEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdAirport).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Airport>> ListActiveAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<AirportEntity>().AsNoTracking();
        query = query.Where(x => x.Active);
        var entities = await query.OrderBy(x => x.IdAirport).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Airport airport, CancellationToken ct = default)
    {
        var entity = ToEntity(airport);
        await _dbContext.Set<AirportEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Airport airport, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AirportEntity>()
            .FirstOrDefaultAsync(x => x.IdAirport == airport.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Airport was not found.");
        }

        var values = ToEntity(airport);
        entity.Name = values.Name;
        entity.IATACode = values.IATACode;
        entity.Active = values.Active;
    }

    public async Task DeleteAsync(AirportId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AirportEntity>().FirstOrDefaultAsync(x => x.IdAirport == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        // Rompe dependencias "suaves" (tabla puente) para evitar error de FK al eliminar.
        // Nota: rutas/vuelos/reservas se manejan como Restrict; si existen, SaveChanges fallará.
        var tzLinks = await _dbContext.Set<AirportTimeZoneEntity>()
            .Where(x => x.IdAirport == id.Value)
            .ToListAsync(ct);
        if (tzLinks.Count > 0)
            _dbContext.Set<AirportTimeZoneEntity>().RemoveRange(tzLinks);

        _dbContext.Set<AirportEntity>().Remove(entity);
    }

    private static Airport ToDomain(AirportEntity entity)
    {
        return Airport.Create(entity.IdAirport, entity.Name, entity.IATACode, entity.IdCity, entity.Active);
    }

    private static AirportEntity ToEntity(Airport aggregate)
    {
        return new AirportEntity
        {
            IdAirport = aggregate.Id.Value,
            Name = aggregate.Name.Value,
            IATACode = aggregate.IATACode.Value,
            IdCity = aggregate.IdCity,
            Active = aggregate.Active
        };
    }
}
