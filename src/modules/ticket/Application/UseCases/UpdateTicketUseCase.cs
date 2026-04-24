using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;

public sealed class UpdateTicketUseCase
{
    private readonly ITicketRepository _repo;
    public UpdateTicketUseCase(ITicketRepository repo) => _repo = repo;

    public async Task<Ticket> ExecuteAsync(int id, string code, DateTime issueDate, int idBooking, int idFare, int idStatus, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(TicketId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Ticket with id '{id}' was not found.");
        if (issueDate > DateTime.Now)
            throw new ArgumentException("La fecha de emisión no puede ser futura.", nameof(issueDate));
        var updated = Ticket.Create(id, code, issueDate, idBooking, idFare, idStatus);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
