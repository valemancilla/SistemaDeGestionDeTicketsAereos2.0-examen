using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.UseCases;

public sealed class GetBoardingPassByIdUseCase
{
    private readonly IBoardingPassRepository _repo;
    public GetBoardingPassByIdUseCase(IBoardingPassRepository repo) => _repo = repo;

    public async Task<BoardingPass> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(BoardingPassId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"No existe pase de abordar con id '{id}'.");
        return entity;
    }
}
