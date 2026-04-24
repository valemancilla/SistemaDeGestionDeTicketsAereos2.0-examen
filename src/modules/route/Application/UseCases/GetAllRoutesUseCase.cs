using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Application.UseCases;

public sealed class GetAllRoutesUseCase
{
    private readonly IRouteRepository _repo;
    public GetAllRoutesUseCase(IRouteRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Route>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
