// =============================================================================
// Tras un pago exitoso que deja la reserva lista: promueve tiquetes de "Activo" a "Emitido"
// para alinear con el flujo del Examen 3 (check-in solo con tiquete emitido).
// No crea tiquetes nuevos: actualiza los ya asociados a la reserva.
// =============================================================================
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.Services;

/// <summary>
/// Regla de negocio: cuando un pago exitoso deja una reserva en estado «Pagada»,
/// emitir automáticamente los tiquetes asociados (estado «Emitido»).
/// </summary>
public sealed class TicketIssuanceAfterPaymentService
{
    private const string TicketEntityType = "Ticket";
    private const string TicketStatusActive = "Activo";
    private const string TicketStatusIssued = "Emitido";

    public async Task EmitTicketsForBookingAsync(AppDbContext context, int idBooking, CancellationToken ct = default)
    {
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        int? activeId = statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, TicketEntityType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, TicketStatusActive, StringComparison.OrdinalIgnoreCase))?.Id.Value;
        int? issuedId = statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, TicketEntityType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, TicketStatusIssued, StringComparison.OrdinalIgnoreCase))?.Id.Value;
        if (activeId is null || issuedId is null)
            return;

        var repo = new TicketRepository(context);
        var all = await new GetAllTicketsUseCase(repo).ExecuteAsync(ct);
        var bookingTickets = all.Where(t => t.IdBooking == idBooking).ToList();
        if (bookingTickets.Count == 0)
            return;

        var updateUc = new UpdateTicketUseCase(repo);
        foreach (var t in bookingTickets)
        {
            if (t.IdStatus != activeId.Value)
                continue; // evitar duplicados y no romper otros estados
            await updateUc.ExecuteAsync(
                t.Id.Value,
                t.Code.Value,
                t.IssueDate.Value,
                t.IdBooking,
                t.IdFare,
                issuedId.Value,
                ct);
        }
    }
}

