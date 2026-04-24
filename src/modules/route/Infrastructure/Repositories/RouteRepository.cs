using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Repositories;

public sealed class RouteRepository : IRouteRepository
{
    private readonly AppDbContext _dbContext;

    public RouteRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Route?> GetByIdAsync(RouteId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<RouteEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdRoute == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Route>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<RouteEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdRoute).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Route>> ListActiveAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<RouteEntity>().AsNoTracking();
        query = query.Where(x => x.Active);
        var entities = await query.OrderBy(x => x.IdRoute).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Route route, CancellationToken ct = default)
    {
        var entity = ToEntity(route);
        await _dbContext.Set<RouteEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Route route, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<RouteEntity>().FirstOrDefaultAsync(x => x.IdRoute == route.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Route was not found.");
        }

        var values = ToEntity(route);
        entity.OriginAirport = values.OriginAirport;
        entity.DestinationAirport = values.DestinationAirport;
        entity.DistanceKm = values.DistanceKm;
        entity.EstDuration = values.EstDuration;
        entity.Active = values.Active;
    }

    public async Task DeleteAsync(RouteId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<RouteEntity>().FirstOrDefaultAsync(x => x.IdRoute == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<RouteEntity>().Remove(entity);
    }

    private static Route ToDomain(RouteEntity entity)
    {
        return Route.Create(entity.IdRoute, entity.DistanceKm, entity.EstDuration, entity.OriginAirport, entity.DestinationAirport, entity.Active);
    }

    private static RouteEntity ToEntity(Route aggregate)
    {
        return new RouteEntity
        {
            IdRoute = aggregate.Id.Value,
            OriginAirport = aggregate.OriginAirport,
            DestinationAirport = aggregate.DestinationAirport,
            DistanceKm = aggregate.DistanceKm.Value,
            EstDuration = aggregate.EstDuration.Value,
            Active = aggregate.Active
        };
    }
}
