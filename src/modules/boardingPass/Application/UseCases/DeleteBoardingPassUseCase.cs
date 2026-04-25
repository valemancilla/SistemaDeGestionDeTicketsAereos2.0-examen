using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.UseCases;

public sealed class DeleteBoardingPassUseCase
{
    private readonly IBoardingPassRepository _repo;
    public DeleteBoardingPassUseCase(IBoardingPassRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var idVo = BoardingPassId.Create(id);
        var existing = await _repo.GetByIdAsync(idVo, ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(idVo, ct);
        return true;
    }
}
