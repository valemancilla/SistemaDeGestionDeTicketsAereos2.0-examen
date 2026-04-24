using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.UseCases;

public sealed class GetBoardingPassByTicketIdUseCase
{
    private readonly IBoardingPassRepository _repo;
    public GetBoardingPassByTicketIdUseCase(IBoardingPassRepository repo) => _repo = repo;

    public Task<BoardingPass?> ExecuteAsync(int idTicket, CancellationToken ct = default) =>
        _repo.GetByTicketIdAsync(idTicket, ct);
}
