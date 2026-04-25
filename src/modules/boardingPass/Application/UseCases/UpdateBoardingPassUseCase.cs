using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.UseCases;

public sealed class UpdateBoardingPassUseCase
{
    private readonly IBoardingPassRepository _repo;
    public UpdateBoardingPassUseCase(IBoardingPassRepository repo) => _repo = repo;

    public async Task<BoardingPass> ExecuteAsync(
        int id,
        string code,
        int idTicket,
        int idSeat,
        string gate,
        DateTime boardingTime,
        DateTime createdAt,
        int idStatus,
        string passengerFullName,
        CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BoardingPassId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"No existe pase de abordar con id '{id}'.");
        var updated = BoardingPass.Create(id, code, idTicket, idSeat, gate, boardingTime, createdAt, idStatus, passengerFullName);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
