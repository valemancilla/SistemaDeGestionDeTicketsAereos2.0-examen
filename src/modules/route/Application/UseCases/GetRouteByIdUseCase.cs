using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Application.UseCases;

public sealed class GetRouteByIdUseCase
{
    private readonly IRouteRepository _repo;
    public GetRouteByIdUseCase(IRouteRepository repo) => _repo = repo;

    public async Task<Route> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(RouteId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Route with id '{id}' was not found.");
        return entity;
    }
}
