using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.UseCases;

public sealed class GetAllGendersUseCase
{
    private readonly IGenderRepository _repo;
    public GetAllGendersUseCase(IGenderRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Gender>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
