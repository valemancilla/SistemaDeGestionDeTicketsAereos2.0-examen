// =============================================================================
// EXAMEN 3 — Check-in y pase de abordar (reglas de negocio en aplicación).
//
// Fases:
//  1) PrepareAsync: localiza tiquete/reserva/vuelo/pasajero; si hay varios pasajeros
//     en la reserva devuelve PassengerChoices sin persistir; valida emitido/pagado/
//     pago aprobado, vuelo (cancelado / vigencia / habilitado), check-in duplicado,
//     ventana horaria y asientos disponibles; devuelve TicketInfo para la UI.
//  2) CompleteAsync: opcional cambio de asiento en BookingCustomer y SeatFlight,
//     crea CheckIn, actualiza tiquete a "Check-in realizado", crea BoardingPass si no existe.
//
// La consola (ClientPnrCheckInMenu) solo pide datos y muestra resultados; no contiene reglas.
// =============================================================================
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;
using CheckInAgg = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.Services;

/// <summary>
/// Servicio de negocio para el flujo del examen de check-in.
/// La UI solo debe recolectar inputs y mostrar outputs: este servicio resuelve datos, valida, persiste y genera pase.
/// </summary>
public sealed class ExamCheckInService
{
    private const string BookingEntityType = "Booking";
    private const string CheckInEntityType = "CheckIn";
    private const string TicketEntityType = "Ticket";
    private const string FlightEntityType = "Flight";
    private const string PaymentEntityType = "Payment";

    private const string BookingStatusPaid = "Pagada";
    private const string CheckInStatusCompleted = "Completado";
    private const string PaymentStatusApproved = "Aprobado";
    private const string TicketStatusIssued = "Emitido";
    private const string TicketStatusCheckInDone = "Check-in realizado";
    private const string BoardingPassStatusGenerated = "Generado";

    // Ventana de tiempo para check-in (examen):
    // - Abre 24h antes de la salida.
    // - Cierra 45 minutos antes de la salida.
    private static readonly TimeSpan CheckInOpensBeforeDeparture = TimeSpan.FromHours(24);
    private static readonly TimeSpan CheckInClosesBeforeDeparture = TimeSpan.FromMinutes(45);

    public enum InputMode
    {
        TicketCode = 1,
        BookingPnr = 2,
        /// <summary>ID numérico de <c>Booking</c> (identificador de reserva en base).</summary>
        BookingId = 3
    }

    public sealed record PrepareRequest(
        InputMode Mode,
        string TicketOrPnrCode,
        string? PassengerSurname,
        int SessionUserRole,
        int SessionUserId,
        int? SessionPersonId,
        /// <summary>Id de <c>BookingCustomer</c> cuando hubo varios pasajeros en la reserva y la UI ya eligió uno.</summary>
        int? SelectedBookingCustomerId = null);

    public sealed record TicketInfo(
        int TicketId,
        string TicketCode,
        string TicketStatusName,
        int BookingId,
        string PassengerFullName,
        string PassengerDocument,
        int PassengerPersonId,
        int BookingCustomerId,
        int FlightId,
        string FlightCode,
        string OriginLabel,
        string DestinationLabel,
        string RouteLabel,
        DateOnly FlightDate,
        TimeOnly FlightDepartureTime,
        DateTime DepartureAtLocal,
        string? BoardingGate,
        int CurrentSeatId);

    public sealed record PrepareResult(
        bool Ok,
        string? ErrorMessage,
        TicketInfo? Info,
        bool SeatRequired,
        IReadOnlyList<(int idSeat, string seatLabel)> SeatChoices,
        /// <summary>Si hay más de un pasajero en la reserva (mismo PNR/apellido), la UI debe elegir y reintentar con <see cref="PrepareRequest.SelectedBookingCustomerId"/>.</summary>
        IReadOnlyList<(int bookingCustomerId, string displayLabel)>? PassengerChoices = null);

    public sealed record CompleteRequest(
        TicketInfo Info,
        int SeatId,
        int SessionUserId);

    public sealed record CompleteResult(
        bool Ok,
        string? ErrorMessage,
        BoardingPass? BoardingPass);

    public async Task<PrepareResult> PrepareAsync(PrepareRequest req, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();

        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);

        int issuedId = RequireStatusId(statuses, TicketEntityType, TicketStatusIssued);
        int checkInDoneId = RequireStatusId(statuses, TicketEntityType, TicketStatusCheckInDone);
        int bookingPaidId = RequireStatusId(statuses, BookingEntityType, BookingStatusPaid);
        int paymentApprovedId = RequireStatusId(statuses, PaymentEntityType, PaymentStatusApproved);

        Ticket? ticket;
        Booking? booking;

        if (req.Mode == InputMode.TicketCode)
        {
            var rawKey = (req.TicketOrPnrCode ?? string.Empty).Trim().ToUpperInvariant();
            ticket = await new TicketRepository(context).GetByCodeAsync(rawKey, ct);
            if (ticket is null)
                return new PrepareResult(false, "Tiquete no encontrado.", null, false, Array.Empty<(int, string)>());

            if (!await UserMayAccessBookingAsync(context, ticket.IdBooking, req.SessionUserRole, req.SessionUserId, req.SessionPersonId, ct))
                return new PrepareResult(false, "Ese tiquete no está asociado a tu sesión.", null, false, Array.Empty<(int, string)>());

            booking = await new BookingRepository(context).GetByIdAsync(BookingId.Create(ticket.IdBooking), ct);
            if (booking is null)
                return new PrepareResult(false, "No se halló la reserva del tiquete.", null, false, Array.Empty<(int, string)>());
        }
        else if (req.Mode == InputMode.BookingId)
        {
            if (!int.TryParse(
                    (req.TicketOrPnrCode ?? string.Empty).Trim(),
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var idBooking) ||
                idBooking <= 0)
                return new PrepareResult(false, "ID de reserva no válido.", null, false, Array.Empty<(int, string)>());

            booking = await new BookingRepository(context).GetByIdAsync(BookingId.Create(idBooking), ct);
            if (booking is null)
                return new PrepareResult(false, "No existe una reserva con ese identificador.", null, false, Array.Empty<(int, string)>());

            if (!await UserMayAccessBookingAsync(context, booking.Id.Value, req.SessionUserRole, req.SessionUserId, req.SessionPersonId, ct))
                return new PrepareResult(false, "Esta reserva no está asociada a tu cuenta.", null, false, Array.Empty<(int, string)>());

            var ticketsByBooking = await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct);
            ticket = ticketsByBooking.Where(t => t.IdBooking == booking.Id.Value).OrderBy(t => t.Id.Value).FirstOrDefault();
            if (ticket is null)
                return new PrepareResult(false, "Tiquete no encontrado.", null, false, Array.Empty<(int, string)>());
        }
        else
        {
            var rawKey = (req.TicketOrPnrCode ?? string.Empty).Trim().ToUpperInvariant();
            booking = await new BookingRepository(context).GetByCodeAsync(rawKey, ct);
            if (booking is null)
                return new PrepareResult(false, "No existe una reserva con ese código.", null, false, Array.Empty<(int, string)>());

            if (!await UserMayAccessBookingAsync(context, booking.Id.Value, req.SessionUserRole, req.SessionUserId, req.SessionPersonId, ct))
                return new PrepareResult(false, "Esta reserva no está asociada a tu cuenta.", null, false, Array.Empty<(int, string)>());

            var tickets = await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct);
            ticket = tickets.Where(t => t.IdBooking == booking.Id.Value).OrderBy(t => t.Id.Value).FirstOrDefault();
            if (ticket is null)
                return new PrepareResult(false, "Tiquete no encontrado.", null, false, Array.Empty<(int, string)>());
        }

        var flight = await new FlightRepository(context).GetByIdAsync(FlightId.Create(booking.IdFlight), ct);
        if (flight is null)
            return new PrepareResult(false, "El vuelo no está disponible", null, false, Array.Empty<(int, string)>());

        // Resolver pasajero (para PNR se filtra por apellido si lo proveen, preservando el flujo actual sin mezclar lógica en UI).
        // Mejora técnica simple: filtrar en repositorio para no traer toda la tabla.
        var links = await new BookingCustomerRepository(context).ListByBookingAsync(booking.Id.Value, ct);
        if (links.Count == 0)
            return new PrepareResult(false, "No hay pasajeros en la reserva.", null, false, Array.Empty<(int, string)>());

        var personRows = await context.Set<PersonEntity>()
            .AsNoTracking()
            .Where(p => links.Select(x => x.IdPerson).Contains(p.IdPerson))
            .ToListAsync(ct);

        var candidates = links
            .Select(l => (link: l, person: personRows.FirstOrDefault(p => p.IdPerson == l.IdPerson)))
            .Where(x => x.person is not null)
            .Select(x => (link: x.link, person: x.person!))
            .ToList();

        if (!string.IsNullOrWhiteSpace(req.PassengerSurname))
            candidates = candidates
                .Where(x => LastNameEquals(x.person.LastName, req.PassengerSurname!))
                .ToList();

        if (candidates.Count == 0)
            return new PrepareResult(false, "No se encontró el tiquete.", null, false, Array.Empty<(int, string)>());

        var seatRepo = new SeatRepository(context);
        var allSeats = await new GetAllSeatsUseCase(seatRepo).ExecuteAsync(ct);
        var seatMap = allSeats.ToDictionary(s => s.Id.Value, s => s);

        if (candidates.Count > 1)
        {
            if (req.SelectedBookingCustomerId is null or 0)
            {
                var passengerChoices = candidates
                    .Select(x =>
                    {
                        var seatTxt = x.link.IdSeat > 0 && seatMap.TryGetValue(x.link.IdSeat, out var sn)
                            ? sn.Number.Value
                            : "sin asiento";
                        var label =
                            $"{x.person.FirstName} {x.person.LastName} (doc. {x.person.DocumentNumber}) — asiento: {seatTxt}";
                        return (x.link.Id.Value, label);
                    })
                    .ToList();
                return new PrepareResult(false, null, null, false, Array.Empty<(int, string)>(), passengerChoices);
            }

            var chosen = candidates.FirstOrDefault(x => x.link.Id.Value == req.SelectedBookingCustomerId.Value);
            if (chosen == default)
                return new PrepareResult(false, "Pasajero no válido para esta reserva.", null, false, Array.Empty<(int, string)>());
            candidates = new List<(BookingCustomer link, PersonEntity person)> { chosen };
        }
        else if (req.SelectedBookingCustomerId is int sel0 && sel0 != 0 && candidates[0].link.Id.Value != sel0)
        {
            return new PrepareResult(false, "Pasajero no válido para esta reserva.", null, false, Array.Empty<(int, string)>());
        }

        var (bookingCustomerLink, passenger) = candidates[0];

        string routeLabel = (await BuildRouteLabelForFlightAsync(context, flight.IdRoute, ct)) ?? "—";
        var routeSplit = routeLabel.Split('→', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var origenTxt = routeSplit.Length > 0 ? routeSplit[0].Trim() : "—";
        var destinoTxt = routeSplit.Length > 1 ? routeSplit[1].Trim() : "—";

        var effectiveTicketStatusId = ticket.IdStatus;
        var ticketStatusName = statuses.FirstOrDefault(s =>
            s.Id.Value == effectiveTicketStatusId &&
            string.Equals(s.EntityType.Value, TicketEntityType, StringComparison.OrdinalIgnoreCase))?.Name.Value ?? "—";

        // IMPORTANTE (Examen 3):
        // PrepareAsync debe ser "puro": SOLO valida y resuelve datos para la UI.
        // No debe cambiar estados, crear registros ni ejecutar SaveChangesAsync.
        //
        // Si el sistema requiere "reparaciones de consistencia" (p.ej. emitir un tiquete cuando hay pago aprobado),
        // eso debe hacerse en un proceso separado (emisión/post-pago) o en un caso de uso explícito,
        // pero NO en el flujo de check-in antes de confirmar.
        //
        // Validación de pago (concepto del enunciado):
        // En este dominio, "Pagado" = Reserva en estado «Pagada» + existencia de un Payment en estado «Aprobado».
        var payments = await new GetAllPaymentsUseCase(new PaymentRepository(context)).ExecuteAsync(ct);
        var hasApprovedPayment = payments.Any(p => p.IdBooking == booking.Id.Value && p.IdStatus == paymentApprovedId);

        // ── VALIDACIONES OBLIGATORIAS (EN ESTE ORDEN) ──────────────────────
        if (effectiveTicketStatusId != issuedId)
            return new PrepareResult(false, "Tiquete no emitido.", null, false, Array.Empty<(int, string)>());

        // Examen 3: el enunciado pide pago "pagado"; en este dominio = reserva «Pagada» + movimiento de pago aprobado.
        if (booking.IdStatus != bookingPaidId)
            return new PrepareResult(
                false,
                "Pago pendiente: la reserva no está en estado «Pagada».",
                null,
                false,
                Array.Empty<(int, string)>());

        if (!hasApprovedPayment)
            return new PrepareResult(
                false,
                "Pago pendiente: no hay un pago «Aprobado» registrado para esta reserva.",
                null,
                false,
                Array.Empty<(int, string)>());

        var departureAtLocal = flight.Date.Value.ToDateTime(flight.DepartureTime.Value);
        var nowLocal = DateTime.Now;
        var flightStatusName = statuses.FirstOrDefault(s =>
            s.Id.Value == flight.IdStatus &&
            string.Equals(s.EntityType.Value, FlightEntityType, StringComparison.OrdinalIgnoreCase))?.Name.Value;

        if (flightStatusName is not null &&
            string.Equals(flightStatusName, "Cancelado", StringComparison.OrdinalIgnoreCase))
            return new PrepareResult(false, "Vuelo cancelado.", null, false, Array.Empty<(int, string)>());

        if (departureAtLocal <= nowLocal)
            return new PrepareResult(false, "El vuelo no está vigente.", null, false, Array.Empty<(int, string)>());

        var allowedFlight = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Programado", "Demorado" };
        if (flightStatusName is null || !allowedFlight.Contains(flightStatusName))
            return new PrepareResult(false, "Vuelo no habilitado.", null, false, Array.Empty<(int, string)>());

        var checkIns = await new GetAllCheckInsUseCase(new CheckInRepository(context)).ExecuteAsync(ct);
        if (effectiveTicketStatusId == checkInDoneId || checkIns.Any(c => c.IdTicket == ticket.Id.Value))
            return new PrepareResult(false, "Check-in ya realizado.", null, false, Array.Empty<(int, string)>());

        var opensAt = departureAtLocal - CheckInOpensBeforeDeparture;
        var closesAt = departureAtLocal - CheckInClosesBeforeDeparture;
        if (nowLocal < opensAt || nowLocal > closesAt)
            return new PrepareResult(false, "Fuera del tiempo permitido.", null, false, Array.Empty<(int, string)>());

        var currentSeatId = bookingCustomerLink.IdSeat;

        var seatChoices = await ListSelectableSeatsAsync(context, flight.Id.Value, currentSeatId, seatMap, ct);

        var info = new TicketInfo(
            ticket.Id.Value,
            ticket.Code.Value,
            ticketStatusName,
            booking.Id.Value,
            $"{passenger.FirstName} {passenger.LastName}".Trim(),
            passenger.DocumentNumber,
            passenger.IdPerson,
            bookingCustomerLink.Id.Value,
            flight.Id.Value,
            flight.Number.Value,
            origenTxt,
            destinoTxt,
            routeLabel,
            flight.Date.Value,
            flight.DepartureTime.Value,
            departureAtLocal,
            flight.BoardingGate,
            currentSeatId
        );

        return new PrepareResult(true, null, info, SeatRequired: currentSeatId <= 0, seatChoices);
    }

    public async Task<CompleteResult> CompleteAsync(CompleteRequest req, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();

        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        int checkInDoneId = RequireStatusId(statuses, TicketEntityType, TicketStatusCheckInDone);
        int checkInCompletedId = RequireStatusId(statuses, CheckInEntityType, CheckInStatusCompleted);
        int bpGeneratedId = RequireStatusId(statuses, "BoardingPass", BoardingPassStatusGenerated);
        _ = RequireStatusId(statuses, "BoardingPass", "Activo"); // validación: existe en catálogo (se usa al abordar)

        // Asiento:
        // - Si el pasajero NO tiene asiento asignado (CurrentSeatId <= 0), el sistema debe autoasignarlo.
        // - Si el usuario envía SeatId (flujo actual), se respeta; si no, se elige el primero disponible.
        int effectiveSeatId = req.SeatId;
        if (effectiveSeatId <= 0)
            effectiveSeatId = req.Info.CurrentSeatId;
        if (effectiveSeatId <= 0)
        {
            var seatRepo = new SeatRepository(context);
            var allSeats = await new GetAllSeatsUseCase(seatRepo).ExecuteAsync(ct);
            var seatMap = allSeats.ToDictionary(s => s.Id.Value, s => s);
            var autoChoices = await ListSelectableSeatsAsync(context, req.Info.FlightId, currentSeatId: 0, seatMap, ct);
            if (autoChoices.Count == 0)
                return new CompleteResult(false, "No hay asientos disponibles para este vuelo.", null);
            effectiveSeatId = autoChoices[0].idSeat;
        }

        // Aplicar asiento si cambia (disponibilidad + vínculo booking-customer)
        if (req.Info.CurrentSeatId != effectiveSeatId)
        {
            var bcRepo = new BookingCustomerRepository(context);
            var legBc = await bcRepo.GetByIdAsync(BookingCustomerId.Create(req.Info.BookingCustomerId), ct)
                ?? throw new InvalidOperationException("No se encontró el registro de pasajero en la reserva.");
            await ApplySeatChangeAsync(context, legBc, req.Info.FlightId, req.Info.CurrentSeatId, effectiveSeatId, ct);
        }

        // Examen 3: estado explícito del pasajero dentro de su reserva tras check-in exitoso.
        {
            var bcRepo = new BookingCustomerRepository(context);
            var legBc = await bcRepo.GetByIdAsync(BookingCustomerId.Create(req.Info.BookingCustomerId), ct)
                ?? throw new InvalidOperationException("No se encontró el registro de pasajero en la reserva.");
            await new UpdateBookingCustomerUseCase(bcRepo).ExecuteAsync(
                legBc.Id.Value,
                legBc.AssociationDate.Value,
                legBc.IdBooking,
                legBc.IdUser,
                legBc.IdPerson,
                effectiveSeatId,
                legBc.IsPrimary,
                isReadyToBoard: true,
                ct: ct);
        }

        const int webChannelId = 1;
        await new CreateCheckInUseCase(new CheckInRepository(context))
            .ExecuteAsync(DateTime.UtcNow, req.Info.TicketId, webChannelId, effectiveSeatId, req.SessionUserId, checkInCompletedId, ct);

        // Re-leer ticket para mantener issue date original
        var ticket = await new GetTicketByIdUseCase(new TicketRepository(context)).ExecuteAsync(req.Info.TicketId, ct);
        await new UpdateTicketUseCase(new TicketRepository(context))
            .ExecuteAsync(ticket.Id.Value, ticket.Code.Value, ticket.IssueDate.Value, ticket.IdBooking, ticket.IdFare, checkInDoneId, ct);

        // Boarding pass (solo si no existe)
        var bpRepo = new BoardingPassRepository(context);
        var existingBp = await new GetBoardingPassByTicketIdUseCase(bpRepo).ExecuteAsync(req.Info.TicketId, ct);
        if (existingBp is null)
        {
            var passCode = $"BP-{Guid.NewGuid():N}".ToUpperInvariant()[..13];
            var gate = string.IsNullOrWhiteSpace(req.Info.BoardingGate) ? "A01" : req.Info.BoardingGate;
            var boardingAt = req.Info.DepartureAtLocal - TimeSpan.FromMinutes(30);
            var passengerName = req.Info.PassengerFullName;

            await new CreateBoardingPassUseCase(bpRepo).ExecuteAsync(
                passCode,
                req.Info.TicketId,
                effectiveSeatId,
                gate,
                boardingAt,
                DateTime.Now,
                bpGeneratedId,
                passengerName,
                ct);
        }

        // Examen 3: trazabilidad de reserva pagada + pago aprobado, tiquete, estado del pasajero (BookingCustomer) y pase.
        await new CreateTicketStatusHistoryUseCase(new TicketStatusHistoryRepository(context))
            .ExecuteAsync(
                DateTime.Now,
                "Examen 3: reserva «Pagada» y pago «Aprobado» verificados. Pasajero: listo para abordar (BookingCustomer.IsReadyToBoard=true) y CheckIn «Completado». Tiquete: Check-in realizado. Pase: Generado.",
                req.Info.TicketId,
                checkInDoneId,
                req.SessionUserId,
                ct);

        await context.SaveChangesAsync(ct);

        var pass = await new GetBoardingPassByTicketIdUseCase(bpRepo).ExecuteAsync(req.Info.TicketId, ct);
        if (pass is null)
            return new CompleteResult(false, "No se pudo generar el pase de abordar.", null);

        return new CompleteResult(true, null, pass);
    }

    public static IReadOnlyList<(int idSeat, string seatLabel)> BuildSeatChoicesForUi(
        IReadOnlyList<(int idSeat, string seatLabel)> seatChoices)
        => seatChoices;

    private static int RequireStatusId(IReadOnlyList<modules.systemStatus.Domain.aggregate.SystemStatus> statuses, string entityType, string name)
    {
        var m = statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, entityType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, name, StringComparison.OrdinalIgnoreCase));
        if (m is null)
            throw new InvalidOperationException($"Faltan estados requeridos en SystemStatus (revisar semillas/migraciones).");
        return m.Id.Value;
    }

    private static bool LastNameEquals(string dbLastName, string input) =>
        string.Equals((dbLastName ?? string.Empty).Trim(), (input ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase);

    private static async Task<bool> UserMayAccessBookingAsync(
        AppDbContext context,
        int idBooking,
        int sessionUserRole,
        int sessionUserId,
        int? sessionPersonId,
        CancellationToken ct)
    {
        if (sessionUserRole == 1)
            return true;

        var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
        foreach (var l in links)
        {
            if (l.IdBooking == idBooking && (l.IdUser == sessionUserId || (sessionPersonId is int pid && l.IdPerson == pid)))
                return true;
        }

        var all = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        var b = all.FirstOrDefault(x => x.Id.Value == idBooking);
        if (b is null)
            return false;
        return ClientBookingCodeOwnership.CodeLooksLikeClientGeneratedBooking(b.Code.Value, sessionUserId);
    }

    private static async Task<string> BuildRouteLabelForFlightAsync(AppDbContext context, int idRoute, CancellationToken ct)
    {
        try
        {
            var routeRepo = new RouteRepository(context);
            var airportRepo = new AirportRepository(context);
            var route = await new GetRouteByIdUseCase(routeRepo).ExecuteAsync(idRoute, ct);
            var origin = await new GetAirportByIdUseCase(airportRepo).ExecuteAsync(route.OriginAirport, ct);
            var dest = await new GetAirportByIdUseCase(airportRepo).ExecuteAsync(route.DestinationAirport, ct);
            return $"{origin.IATACode.Value} → {dest.IATACode.Value}";
        }
        catch
        {
            return "—";
        }
    }

    private static async Task<IReadOnlyList<(int idSeat, string seatLabel)>> ListSelectableSeatsAsync(
        AppDbContext context,
        int idFlight,
        int currentSeatId,
        IReadOnlyDictionary<int, Seat> seatMap,
        CancellationToken ct)
    {
        var seatFlights = await new GetAllSeatFlightsUseCase(new SeatFlightRepository(context)).ExecuteAsync(ct);
        var selectableIds = seatFlights
            .Where(sf => sf.IdFlight == idFlight && (sf.Available || sf.IdSeat == currentSeatId))
            .Select(sf => sf.IdSeat)
            .Distinct()
            .OrderBy(id => id)
            .ToList();

        return selectableIds
            .Where(id => seatMap.ContainsKey(id))
            .Select(id => (id, seatMap[id].Number.Value))
            .ToList();
    }

    private static async Task ApplySeatChangeAsync(
        AppDbContext context,
        BookingCustomer legBc,
        int idFlight,
        int oldSeatId,
        int newSeatId,
        CancellationToken ct)
    {
        if (oldSeatId == newSeatId)
            return;

        var sfRepo = new SeatFlightRepository(context);
        var updateSf = new UpdateSeatFlightUseCase(sfRepo);
        if (oldSeatId > 0)
        {
            var oldSf = await sfRepo.GetBySeatAndFlightAsync(oldSeatId, idFlight, ct);
            if (oldSf is not null)
                await updateSf.ExecuteAsync(oldSf.Id.Value, oldSeatId, idFlight, available: true, ct);
        }

        var newSf = await sfRepo.GetBySeatAndFlightAsync(newSeatId, idFlight, ct)
            ?? throw new InvalidOperationException("El asiento elegido no pertenece a este vuelo.");
        if (!newSf.Available && newSf.IdSeat != oldSeatId)
            throw new InvalidOperationException("El asiento ya no está disponible.");

        await updateSf.ExecuteAsync(newSf.Id.Value, newSeatId, idFlight, available: false, ct);

        var bcRepo = new BookingCustomerRepository(context);
        var existing = await bcRepo.GetByIdAsync(BookingCustomerId.Create(legBc.Id.Value), ct)
            ?? throw new InvalidOperationException("No se encontró el registro de pasajero en la reserva.");

        var updated = BookingCustomer.Create(
            existing.Id.Value,
            existing.AssociationDate.Value,
            existing.IdBooking,
            existing.IdUser,
            existing.IdPerson,
            newSeatId,
            existing.IsPrimary);
        await new UpdateBookingCustomerUseCase(bcRepo).ExecuteAsync(
            updated.Id.Value,
            updated.AssociationDate.Value,
            updated.IdBooking,
            updated.IdUser,
            updated.IdPerson,
            updated.IdSeat,
            updated.IsPrimary,
            ct);
    }
}

