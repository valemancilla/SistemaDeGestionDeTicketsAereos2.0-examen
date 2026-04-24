using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Application.UseCases;

public sealed class UpdateRouteUseCase
{
    private readonly IRouteRepository _repo;
    public UpdateRouteUseCase(IRouteRepository repo) => _repo = repo;

    public async Task<Route> ExecuteAsync(int id, decimal distanceKm, TimeOnly estDuration, int originAirport, int destinationAirport, bool active, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(RouteId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Route with id '{id}' was not found.");
        var updated = Route.Create(id, distanceKm, estDuration, originAirport, destinationAirport, active);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
