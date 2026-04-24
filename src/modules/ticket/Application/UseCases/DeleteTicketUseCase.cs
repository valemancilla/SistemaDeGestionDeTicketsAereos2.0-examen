using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;

public sealed class DeleteTicketUseCase
{
    private readonly ITicketRepository _repo;
    public DeleteTicketUseCase(ITicketRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(TicketId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(TicketId.Create(id), ct);
        return true;
    }
}
