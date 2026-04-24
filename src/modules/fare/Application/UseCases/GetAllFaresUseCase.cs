using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.UseCases;

public sealed class GetAllFaresUseCase
{
    private readonly IFareRepository _repo;
    public GetAllFaresUseCase(IFareRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Fare>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
