using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.UseCases;

public sealed class GetAllTicketStatusHistoriesUseCase
{
    private readonly ITicketStatusHistoryRepository _repo;
    public GetAllTicketStatusHistoriesUseCase(ITicketStatusHistoryRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<TicketStatusHistory>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
