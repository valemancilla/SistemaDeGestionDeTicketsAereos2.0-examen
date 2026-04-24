using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.UseCases;

public sealed class UpdateTicketStatusHistoryUseCase
{
    private readonly ITicketStatusHistoryRepository _repo;
    public UpdateTicketStatusHistoryUseCase(ITicketStatusHistoryRepository repo) => _repo = repo;

    public async Task<TicketStatusHistory> ExecuteAsync(int id, DateTime changeDate, string? observation, int idTicket, int idStatus, int idUser, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(TicketStatusHistoryId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"TicketStatusHistory with id '{id}' was not found.");
        var updated = TicketStatusHistory.Create(id, changeDate, observation, idTicket, idStatus, idUser);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
