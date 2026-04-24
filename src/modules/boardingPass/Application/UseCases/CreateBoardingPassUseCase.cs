using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.UseCases;

public sealed class CreateBoardingPassUseCase
{
    private readonly IBoardingPassRepository _repo;
    public CreateBoardingPassUseCase(IBoardingPassRepository repo) => _repo = repo;

    public async Task<BoardingPass> ExecuteAsync(
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
        var entity = BoardingPass.CreateNew(code, idTicket, idSeat, gate, boardingTime, createdAt, idStatus, passengerFullName);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}

