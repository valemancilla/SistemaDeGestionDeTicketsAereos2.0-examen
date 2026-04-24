using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;

public sealed class GetAllTicketsUseCase
{
    private readonly ITicketRepository _repo;
    public GetAllTicketsUseCase(ITicketRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Ticket>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
