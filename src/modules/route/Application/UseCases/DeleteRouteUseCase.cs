using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Application.UseCases;

public sealed class DeleteRouteUseCase
{
    private readonly IRouteRepository _repo;
    public DeleteRouteUseCase(IRouteRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(RouteId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(RouteId.Create(id), ct);
        return true;
    }
}
