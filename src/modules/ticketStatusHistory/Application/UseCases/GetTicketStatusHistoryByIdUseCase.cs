using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.UseCases;

public sealed class GetTicketStatusHistoryByIdUseCase
{
    private readonly ITicketStatusHistoryRepository _repo;
    public GetTicketStatusHistoryByIdUseCase(ITicketStatusHistoryRepository repo) => _repo = repo;

    public async Task<TicketStatusHistory> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(TicketStatusHistoryId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"TicketStatusHistory with id '{id}' was not found.");
        return entity;
    }
}
