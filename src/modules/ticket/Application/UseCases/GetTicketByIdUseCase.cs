using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;

public sealed class GetTicketByIdUseCase
{
    private readonly ITicketRepository _repo;
    public GetTicketByIdUseCase(ITicketRepository repo) => _repo = repo;

    public async Task<Ticket> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(TicketId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Ticket with id '{id}' was not found.");
        return entity;
    }
}
