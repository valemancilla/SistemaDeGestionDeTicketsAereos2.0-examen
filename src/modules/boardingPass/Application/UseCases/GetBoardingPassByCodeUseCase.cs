using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.UseCases;

public sealed class GetBoardingPassByCodeUseCase
{
    private readonly IBoardingPassRepository _repo;
    public GetBoardingPassByCodeUseCase(IBoardingPassRepository repo) => _repo = repo;

    public Task<BoardingPass?> ExecuteAsync(string code, CancellationToken ct = default) =>
        _repo.GetByCodeAsync(code, ct);
}
