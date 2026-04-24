using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;

public sealed class CreateTicketUseCase
{
    private readonly ITicketRepository _repo;
    public CreateTicketUseCase(ITicketRepository repo) => _repo = repo;

    /// <param name="idBooking">Reserva a la que se asocia el tiquete.</param>
    /// <remarks>La fecha/hora de emisión la fija el agregado con un único <see cref="DateTime.Now"/>, no la capa de aplicación.</remarks>
    public async Task<Ticket> ExecuteAsync(string code, int idBooking, int idFare, int idStatus, CancellationToken ct = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var existing = await _repo.GetByCodeAsync(normalizedCode, ct);
        if (existing is not null) throw new InvalidOperationException($"Ticket with code '{normalizedCode}' already exists.");
        var entity = Ticket.CreateNew(normalizedCode, idBooking, idFare, idStatus);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
