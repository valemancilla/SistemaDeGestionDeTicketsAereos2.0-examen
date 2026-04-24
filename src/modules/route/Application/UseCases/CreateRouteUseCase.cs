using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Application.UseCases;

public sealed class CreateRouteUseCase
{
    private readonly IRouteRepository _repo;
    public CreateRouteUseCase(IRouteRepository repo) => _repo = repo;

    public async Task<Route> ExecuteAsync(decimal distanceKm, TimeOnly estDuration, int originAirport, int destinationAirport, bool active, CancellationToken ct = default)
    {
        var entity = Route.CreateNew(distanceKm, estDuration, originAirport, destinationAirport, active);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
