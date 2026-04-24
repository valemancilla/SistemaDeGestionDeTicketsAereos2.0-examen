using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.UseCases;

public sealed class DeleteTicketStatusHistoryUseCase
{
    private readonly ITicketStatusHistoryRepository _repo;
    public DeleteTicketStatusHistoryUseCase(ITicketStatusHistoryRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(TicketStatusHistoryId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(TicketStatusHistoryId.Create(id), ct);
        return true;
    }
}
