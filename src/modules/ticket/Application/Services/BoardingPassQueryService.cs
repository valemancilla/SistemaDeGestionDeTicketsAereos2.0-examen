// =============================================================================
// EXAMEN 3 — Consulta de pase de abordar (capa aplicación).
// - Modos: código de tiquete, código de pase, documento del cliente.
// - Cliente: solo pases asociados a sus tiquetes de reservas pagadas; documento
//   debe coincidir con la persona vinculada al perfil cuando aplica.
// - Si hay varios pases para el mismo documento, devuelve PassChoices para que la UI elija.
// =============================================================================
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.Services;

/// <summary>
/// Servicio de consulta del pase de abordar (Examen 3).
/// Administra permisos por rol y desambiguación cuando un documento tiene más de un pase generado.
/// </summary>
public sealed class BoardingPassQueryService
{
    public enum QueryMode
    {
        TicketCode = 1,
        BoardingPassCode = 2,
        DocumentNumber = 3
    }

    public sealed record QueryRequest(
        QueryMode Mode,
        string Input,
        int SessionUserRole,
        int SessionUserId,
        int? SessionPersonId);

    /// <param name="PassChoices">Si la búsqueda por documento devuelve varios pases, la UI debe dejar elegir uno (mismo criterio de acceso).</param>
    public sealed record QueryResult(
        bool Ok,
        string? ErrorMessage,
        BoardingPass? BoardingPass,
        IReadOnlyList<BoardingPass>? PassChoices = null);

    public async Task<QueryResult> FindAsync(QueryRequest req, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var bpRepo = new BoardingPassRepository(context);

        BoardingPass? bp = null;

        if (req.Mode == QueryMode.TicketCode)
        {
            var code = (req.Input ?? string.Empty).Trim().ToUpperInvariant();
            var ticket = await new TicketRepository(context).GetByCodeAsync(code, ct);
            if (ticket is not null)
            {
                if (req.SessionUserRole != 1)
                {
                    var owned = await GetMyTicketIdsAsync(context, req.SessionUserRole, req.SessionUserId, req.SessionPersonId, ct);
                    if (!owned.Contains(ticket.Id.Value))
                        return new QueryResult(false, "Ese tiquete no está asociado a tu perfil. Usá un código de un tiquete tuyo.", null, null);
                }

                bp = await new GetBoardingPassByTicketIdUseCase(bpRepo).ExecuteAsync(ticket.Id.Value, ct);
            }
        }
        else if (req.Mode == QueryMode.BoardingPassCode)
        {
            var raw = req.Input ?? string.Empty;
            bp = await new GetBoardingPassByCodeUseCase(bpRepo).ExecuteAsync(raw, ct);
        }
        else
        {
            var docInput = (req.Input ?? string.Empty).Trim();
            var normalized = PersonDocumentNumber.Create(docInput).Value;

            var personRepo = new PersonRepository(context);
            var person = await personRepo.GetByDocumentNumberAsync(normalized, ct);
            if (person is null)
                return new QueryResult(false, "No se encontró pase de abordar. Verificá: reserva Pagada, check-in completado y, si aplica, que el tiquete sea tuyo.", null, null);

            if (req.SessionUserRole != 1)
            {
                if (req.SessionPersonId is not int sessionPersonId)
                    return new QueryResult(false, "Usá Código de tiquete/pase o completá el perfil: tu cuenta no tiene persona/documento vinculado.", null, null);
                if (sessionPersonId != person.Id.Value)
                    return new QueryResult(false, "Solo podés buscar pases con el mismo documento de tu perfil.", null, null);
            }

            var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
            var bookingIdSet = new HashSet<int>(links
                .Where(l => l.IdPerson == person.Id.Value)
                .Select(l => l.IdBooking));
            if (req.SessionPersonId is int ap && ap == person.Id.Value)
            {
                foreach (var l in links.Where(l => l.IdUser == req.SessionUserId))
                    bookingIdSet.Add(l.IdBooking);
            }
            var bookingIds = bookingIdSet.ToList();

            var tickets = await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct);
            var ticketIds = tickets.Where(t => bookingIds.Contains(t.IdBooking)).Select(t => t.Id.Value).Distinct().ToList();
            if (req.SessionUserRole != 1)
            {
                var owned = await GetMyTicketIdsAsync(context, req.SessionUserRole, req.SessionUserId, req.SessionPersonId, ct);
                ticketIds = ticketIds.Where(owned.Contains).ToList();
            }

            var passes = new List<BoardingPass>();
            foreach (var tid in ticketIds)
            {
                var p = await new GetBoardingPassByTicketIdUseCase(bpRepo).ExecuteAsync(tid, ct);
                if (p is not null) passes.Add(p);
            }

            if (passes.Count == 0)
                bp = null;
            else if (req.SessionUserRole != 1)
            {
                var myTicketIds = await GetMyTicketIdsAsync(context, req.SessionUserRole, req.SessionUserId, req.SessionPersonId, ct);
                var mine = passes.Where(p => myTicketIds.Contains(p.IdTicket)).ToList();
                if (mine.Count == 0)
                    return new QueryResult(false, "No se encontró pase de abordar. Verificá: reserva Pagada, check-in completado y, si aplica, que el tiquete sea tuyo.", null, null);
                if (mine.Count > 1)
                    return new QueryResult(false, null, null, mine);
                bp = mine[0];
            }
            else if (passes.Count > 1)
                return new QueryResult(false, null, null, passes);
            else
                bp = passes[0];
        }

        if (bp is not null && req.SessionUserRole != 1)
        {
            var myTicketIds = await GetMyTicketIdsAsync(context, req.SessionUserRole, req.SessionUserId, req.SessionPersonId, ct);
            if (!myTicketIds.Contains(bp.IdTicket))
                return new QueryResult(false, "Ese pase no corresponde a los tiquetes vinculados a tu perfil.", null, null);
        }

        if (bp is null)
            return new QueryResult(false, "No se encontró pase de abordar. Verificá: reserva Pagada, check-in completado y, si aplica, que el tiquete sea tuyo.", null, null);

        return new QueryResult(true, null, bp, null);
    }

    private static async Task<HashSet<int>> GetMyTicketIdsAsync(
        AppDbContext context,
        int sessionUserRole,
        int sessionUserId,
        int? sessionPersonId,
        CancellationToken ct)
    {
        if (sessionUserRole == 1)
            return new HashSet<int>();

        var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
        var myBookingIds = new HashSet<int>();
        foreach (var l in links)
        {
            if (l.IdUser == sessionUserId)
                myBookingIds.Add(l.IdBooking);
            if (sessionPersonId is int pid && l.IdPerson == pid)
                myBookingIds.Add(l.IdBooking);
        }

        var allBookings = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        foreach (var b in allBookings)
        {
            if (ClientBookingCodeOwnership.CodeLooksLikeClientGeneratedBooking(b.Code.Value, sessionUserId))
                myBookingIds.Add(b.Id.Value);
        }

        // Solo reservas pagadas en este sistema
        var statuses = await new modules.systemStatus.Application.UseCases.GetAllSystemStatusesUseCase(new modules.systemStatus.Infrastructure.Repositories.SystemStatusRepository(context)).ExecuteAsync(ct);
        var paidStatus = statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, "Booking", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, "Pagada", StringComparison.OrdinalIgnoreCase));
        if (paidStatus is null)
            return new HashSet<int>();

        var paidBookingIds = allBookings
            .Where(b => b.IdStatus == paidStatus.Id.Value && myBookingIds.Contains(b.Id.Value))
            .Select(b => b.Id.Value)
            .ToHashSet();

        var tickets = await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct);
        return tickets
            .Where(t => paidBookingIds.Contains(t.IdBooking))
            .Select(t => t.Id.Value)
            .ToHashSet();
    }
}

