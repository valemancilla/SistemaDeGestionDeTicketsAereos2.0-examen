using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Domain;
using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Infrastructure.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Application.UseCases;

public sealed class GetClientFareBundleDisplayUseCase
{
    private readonly ClientFareBundleDisplayRepository _repo;
    public GetClientFareBundleDisplayUseCase(ClientFareBundleDisplayRepository repo) => _repo = repo;

    public Task<ClientFareBundleDisplayData> ExecuteAsync(CancellationToken ct = default)
        => _repo.GetOrCreateSingletonAsync(ct);
}
