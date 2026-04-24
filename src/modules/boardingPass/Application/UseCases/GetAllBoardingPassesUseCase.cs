using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.UseCases;

public sealed class GetAllBoardingPassesUseCase
{
    private readonly IBoardingPassRepository _repo;
    public GetAllBoardingPassesUseCase(IBoardingPassRepository repo) => _repo = repo;

    public Task<IReadOnlyList<BoardingPass>> ExecuteAsync(CancellationToken ct = default) =>
        _repo.ListAsync(ct);
}
