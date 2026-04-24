using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.UseCases;

public sealed class CreateTicketStatusHistoryUseCase
{
    private readonly ITicketStatusHistoryRepository _repo;
    public CreateTicketStatusHistoryUseCase(ITicketStatusHistoryRepository repo) => _repo = repo;

    public async Task<TicketStatusHistory> ExecuteAsync(DateTime changeDate, string? observation, int idTicket, int idStatus, int idUser, CancellationToken ct = default)
    {
        var entity = TicketStatusHistory.CreateNew(changeDate, observation, idTicket, idStatus, idUser);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
