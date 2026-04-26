// =============================================================================
// EXAMEN 3 — UI de "Realizar check-in" (cliente).
// - Consistencia enunciado: vuelo vs asiento, pagos, ciclo pase, sin cambiar reglas (ExamCheckInService).
// - Entrada: código de tiquete, PNR o ID numérico de reserva (+ apellido si aplica).
// - Delegación: ExamCheckInService (PrepareAsync → posible elección de pasajero →
//   panel de datos → asiento si falta → CompleteAsync).
// - Salida: pase impreso en consola con estados Generado (BD) y texto de uso Activo.
// Otros métodos estáticos del archivo conservan utilidades de resúmenes de viaje / legado.
// =============================================================================
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Infrastructure.Repositories;
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
using CheckInAgg = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.Services;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.UI;

public static class ClientPnrCheckInMenu
{
    // Nota: la ventana horaria y validaciones del examen viven en ExamCheckInService (negocio),
    // la UI solo recolecta inputs y presenta outputs.

    public static async Task RunAsync(CancellationToken ct = default)
    {
        await RunExamCheckInFlowAsync(ct);
    }

    /// <summary>
    /// Flujo del examen (requerimiento): buscar tiquete o reserva, mostrar datos, validar en orden exacto y,
    /// solo si todo es correcto, registrar check-in + actualizar estado del tiquete + generar pase.
    /// </summary>
    private static async Task RunExamCheckInFlowAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]REALIZAR CHECK-IN[/]").Centered());

        var modo = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Ingrese:")
                .PageSize(5)
                .AddChoices(
                    "1. Código de tiquete",
                    "2. Código de reserva (PNR)",
                    "3. ID de reserva (número)"));

        var ticketRepoInput = AnsiConsole.Prompt(
            new TextPrompt<string>(modo.StartsWith("1", StringComparison.Ordinal)
                ? "Código del tiquete:"
                : modo.StartsWith("2", StringComparison.Ordinal)
                    ? "Código de reserva (PNR):"
                    : "ID de reserva (número):")
                .Validate(v => string.IsNullOrWhiteSpace(v)
                    ? ValidationResult.Error("[red]El dato no puede estar vacío.[/]")
                    : ValidationResult.Success()));

        // PNR o ID de reserva: apellido para filtrar / elegir pasajero cuando la reserva trae a varios.
        string? surname = null;
        if (modo.StartsWith("2", StringComparison.Ordinal) || modo.StartsWith("3", StringComparison.Ordinal))
        {
            surname = AnsiConsole.Prompt(
                new TextPrompt<string>("Apellido del pasajero:")
                    .Validate(v => string.IsNullOrWhiteSpace(v)
                        ? ValidationResult.Error("[red]El apellido no puede estar vacío.[/]")
                        : ValidationResult.Success()));
        }

        try
        {
            var service = new ExamCheckInService();
            var mode = modo.StartsWith("1", StringComparison.Ordinal)
                ? ExamCheckInService.InputMode.TicketCode
                : modo.StartsWith("2", StringComparison.Ordinal)
                    ? ExamCheckInService.InputMode.BookingPnr
                    : ExamCheckInService.InputMode.BookingId;

            int? selectedBookingCustomerId = null;
            ExamCheckInService.PrepareResult prepared;
            while (true)
            {
                prepared = await service.PrepareAsync(
                    new ExamCheckInService.PrepareRequest(
                        mode,
                        ticketRepoInput,
                        surname,
                        AppState.IdUserRole,
                        AppState.IdUser,
                        AppState.IdPerson,
                        selectedBookingCustomerId),
                    ct);

                if (!prepared.Ok && prepared.PassengerChoices is { Count: > 0 } && prepared.ErrorMessage is null)
                {
                    var choices = prepared.PassengerChoices
                        .Select(c => $"{c.bookingCustomerId}. {c.displayLabel}")
                        .ToList();
                    var pick = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Varios pasajeros coinciden. Elegí quién hace el check-in:")
                            .PageSize(10)
                            .AddChoices(choices));
                    var d = pick.IndexOf('.');
                    selectedBookingCustomerId = int.Parse(pick.AsSpan(0, d), CultureInfo.InvariantCulture);
                    continue;
                }

                break;
            }

            if (!prepared.Ok || prepared.Info is null)
            {
                // Mensajes del servicio: validaciones del enunciado (texto plano) y otros (Markup rojo).
                if (prepared.ErrorMessage is not null &&
                    (prepared.ErrorMessage.StartsWith("Tiquete no emitido", StringComparison.Ordinal) ||
                     prepared.ErrorMessage.StartsWith("Pago pendiente", StringComparison.Ordinal) ||
                     prepared.ErrorMessage.StartsWith("Vuelo cancelado", StringComparison.Ordinal) ||
                     prepared.ErrorMessage.StartsWith("Vuelo no habilitado", StringComparison.Ordinal) ||
                     prepared.ErrorMessage.StartsWith("El vuelo no está vigente", StringComparison.Ordinal) ||
                     prepared.ErrorMessage.StartsWith("Check-in ya realizado", StringComparison.Ordinal) ||
                     prepared.ErrorMessage.StartsWith("Fuera del tiempo permitido", StringComparison.Ordinal) ||
                     prepared.ErrorMessage.StartsWith("El tiquete", StringComparison.Ordinal) ||
                     prepared.ErrorMessage.StartsWith("El pago", StringComparison.Ordinal) ||
                     prepared.ErrorMessage.StartsWith("El vuelo", StringComparison.Ordinal) ||
                     prepared.ErrorMessage.StartsWith("El check-in", StringComparison.Ordinal) ||
                     prepared.ErrorMessage.StartsWith("No está", StringComparison.Ordinal)))
                {
                    AnsiConsole.WriteLine(prepared.ErrorMessage);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]{Markup.Escape(prepared.ErrorMessage ?? "Error")}[/]");
                }
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
                return;
            }

            var info = prepared.Info;

            var gateVuelo = string.IsNullOrWhiteSpace(info.BoardingGate) ? "—" : info.BoardingGate;
            var asientoLinea = prepared.SeatRequired
                ? "Sin asiento aún: elegí uno en el paso siguiente (plazas del vuelo vía reserva)."
                : $"Asiento asignado al pasajero en la reserva (id {info.CurrentSeatId}).";
            AnsiConsole.Write(new Panel(
                    $"[bold]Nombre del pasajero:[/] {Markup.Escape(info.PassengerFullName)}\n" +
                    $"[bold]Documento del pasajero:[/] {Markup.Escape(info.PassengerDocument)}\n" +
                    $"[bold]Código del vuelo:[/] {Markup.Escape(info.FlightCode)} [dim](itinerario / vuelo)[/]\n" +
                    $"[bold]Origen:[/] {Markup.Escape(info.OriginLabel)} [dim]·[/] [bold]Destino:[/] {Markup.Escape(info.DestinationLabel)} [dim](ruta del vuelo)[/]\n" +
                    $"[bold]Fecha y hora de salida:[/] {info.FlightDate:yyyy-MM-dd} {info.FlightDepartureTime:hh\\:mm} [dim](programación del vuelo)[/]\n" +
                    $"[bold]Puerta de embarque:[/] {Markup.Escape(gateVuelo)} [dim](dato del vuelo)[/]\n" +
                    $"[bold]Asiento (reserva + plazas del vuelo):[/] {Markup.Escape(asientoLinea)}\n" +
                    $"[bold]Estado actual del tiquete:[/] {Markup.Escape(info.TicketStatusName)}")
                .Header("[cyan]Información del tiquete[/]")
                .Border(BoxBorder.Rounded));
            AnsiConsole.MarkupLine(
                "[dim]Nota (Examen 3): ruta, salida y puerta provienen del vuelo; el asiento figura en la reserva del pasajero y en el mapa de asientos del vuelo, no como columna del vuelo.[/]");
            AnsiConsole.MarkupLine(
                "[green]Pago validado correctamente[/] [dim](Pagado = Reserva «Pagada» + Pago «Aprobado»).[/]");

            int seatId = info.CurrentSeatId;
            string seatLabel = seatId.ToString(CultureInfo.InvariantCulture);
            if (prepared.SeatRequired)
            {
                var choices = prepared.SeatChoices
                    .Select(c => $"{c.idSeat}. {c.seatLabel}")
                    .ToList();
                if (choices.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]No hay asientos disponibles para este vuelo.[/]");
                    ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
                    return;
                }
                string sel = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Elegí asiento:")
                        .PageSize(15)
                        .AddChoices(choices));
                var dSel = sel.IndexOf('.');
                seatId = int.Parse(sel.AsSpan(0, dSel), CultureInfo.InvariantCulture);
                seatLabel = sel[(dSel + 1)..].Trim();
            }

            if (!AnsiConsole.Confirm("¿Confirmar check-in?", true))
            {
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
                return;
            }

            var completed = await service.CompleteAsync(
                new ExamCheckInService.CompleteRequest(info, seatId, AppState.IdUser),
                ct);

            if (!completed.Ok || completed.BoardingPass is null)
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(completed.ErrorMessage ?? "Error")}[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
                return;
            }

            var pass = completed.BoardingPass;
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("=====================================");
            AnsiConsole.WriteLine("PASE DE ABORDAR");
            AnsiConsole.WriteLine("===============");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine($"Pasajero: {info.PassengerFullName}");
            AnsiConsole.WriteLine($"Documento: {info.PassengerDocument}");
            AnsiConsole.WriteLine($"Vuelo: {info.FlightCode}");
            AnsiConsole.WriteLine($"Ruta: {info.RouteLabel}");
            AnsiConsole.WriteLine($"Asiento: {seatLabel}");
            AnsiConsole.WriteLine($"Puerta: {pass.Gate.Value}");
            AnsiConsole.WriteLine($"Hora de abordaje: {pass.BoardingTime:yyyy-MM-dd HH:mm}");
            AnsiConsole.WriteLine("Estado del pase en base de datos: Generado");
            AnsiConsole.WriteLine(
                "El pase pasa a «Activo» al registrar abordaje (menú admin: Registrar abordaje). " +
                "Hasta entonces, presentate en puerta con este pase y el tiquete en Check-in realizado.");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine($"Código pase: {pass.Code.Value}");
            AnsiConsole.WriteLine("=====================================");

            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
        }
    }

    private static (BookingCustomer link, PersonEntity person) PickPassengerInteractive(
        IReadOnlyList<(BookingCustomer link, PersonEntity person)> candidates)
    {
        var choices = candidates
            .Select((m, i) =>
                $"{i + 1}. {m.person.FirstName} {m.person.LastName} (doc. {m.person.DocumentNumber})")
            .ToList();
        string pick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Elegí el pasajero que hace el check-in:")
                .PageSize(10)
                .AddChoices(choices));
        var d = pick.IndexOf('.');
        var idx = int.Parse(pick.AsSpan(0, d), CultureInfo.InvariantCulture) - 1;
        idx = Math.Clamp(idx, 0, candidates.Count - 1);
        return candidates[idx];
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

    /// <summary>Resumen del viaje: ida y vuelta (ambos tramos en un panel) o solo ida (un tramo).</summary>
    private static void RenderTripSummaryPanel(
        PersonEntity chosenPerson,
        IReadOnlyList<(Booking bookingLeg, Flight flightLeg, string routeLabel, string legKindLabel)> legs,
        IReadOnlyDictionary<int, Ticket> ticketByBooking,
        IReadOnlyList<CheckInAgg> checkIns)
    {
        var isRoundTrip = legs.Count > 1;
        var title = isRoundTrip ? "Tu viaje — Ida y vuelta" : "Tu viaje — Solo ida";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"[bold]Pasajero:[/] {Markup.Escape(chosenPerson.FirstName)} {Markup.Escape(chosenPerson.LastName)}");
        if (isRoundTrip)
        {
            sb.AppendLine(
                "[grey]Ambos tramos están [bold]pagados[/] y con [bold]tiquete[/]. " +
                "Los datos de ida y regreso aparecen juntos; el check-in lo hacés [bold]por tramo[/] (elegís cuál a continuación).[/]");
        }
        else
        {
            sb.AppendLine(
                "[grey]Un solo vuelo [bold]pagado[/] con [bold]tiquete[/]. " +
                "Seguimos con asiento, equipaje y confirmación de check-in para este trayecto.[/]");
        }

        sb.AppendLine();

        for (var i = 0; i < legs.Count; i++)
        {
            var leg = legs[i];
            var bk = leg.bookingLeg;
            var fl = leg.flightLeg;
            ticketByBooking.TryGetValue(bk.Id.Value, out var tk);
            var ticketId = tk?.Id.Value ?? 0;
            var checkDone = ticketId > 0 && checkIns.Any(c => c.IdTicket == ticketId);
            var ticketCode = tk is null ? "—" : Markup.Escape(tk.Code.Value);
            var chk = checkDone ? "Realizado" : "Pendiente";

            if (isRoundTrip)
                sb.AppendLine($"[bold cyan]{Markup.Escape(leg.legKindLabel)}[/]");

            sb.AppendLine($"  [bold]PNR:[/] {Markup.Escape(bk.Code.Value)}");
            sb.AppendLine($"  [bold]Vuelo:[/] {Markup.Escape(fl.Number.Value)} · [bold]Ruta:[/] {Markup.Escape(leg.routeLabel)}");
            sb.AppendLine($"  [bold]Salida:[/] {fl.Date.Value:yyyy-MM-dd} {fl.DepartureTime.Value:hh\\:mm}");
            sb.AppendLine($"  [bold]Plazas reservadas:[/] {bk.SeatCount.Value}");
            sb.AppendLine($"  [bold]Estado de la reserva:[/] Pagada");
            sb.AppendLine($"  [bold]Tiquete:[/] {ticketCode}");
            sb.AppendLine($"  [bold]Check-in de este tramo:[/] {chk}");
            if (isRoundTrip && i < legs.Count - 1)
                sb.AppendLine();
        }

        AnsiConsole.Write(new Panel(sb.ToString())
            .Header($"[green]{title}[/]")
            .Border(BoxBorder.Rounded));
        AnsiConsole.WriteLine();
    }

    /// <summary>Formato requerido por el examen (consola de texto).</summary>
    private static void PrintExamStyleBoardingPass(
        BoardingPass pass,
        string flightNumber,
        string routeLabel,
        string seatLabel,
        string passStateName)
    {
        string passenger = string.IsNullOrWhiteSpace(pass.PassengerFullName) ? "—" : pass.PassengerFullName;
        var stateLine = string.IsNullOrWhiteSpace(passStateName) ? "Generado" : passStateName;
        AnsiConsole.WriteLine("=====================================");
        AnsiConsole.WriteLine("PASE DE ABORDAR");
        AnsiConsole.WriteLine("===============");
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Pasajero: {passenger}");
        AnsiConsole.WriteLine($"Vuelo: {flightNumber}");
        AnsiConsole.WriteLine($"Ruta: {routeLabel}");
        AnsiConsole.WriteLine($"Asiento: {seatLabel}");
        AnsiConsole.WriteLine($"Puerta: {pass.Gate.Value}");
        AnsiConsole.WriteLine($"Hora de abordaje: {pass.BoardingTime:yyyy-MM-dd HH:mm}");
        AnsiConsole.WriteLine($"Estado: {stateLine}");
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Código pase: {pass.Code.Value}");
        AnsiConsole.WriteLine("=====================================");
        AnsiConsole.WriteLine("Check-in realizado con exito");
        AnsiConsole.WriteLine("==============================");
    }

    // Nota: asignación de asientos y persistencia del check-in/pase se movieron a ExamCheckInService (Application/Services).

    private static async Task<List<(Booking bookingLeg, Flight flightLeg, string routeLabel, string legKindLabel)>>
        BuildPaidLegsForPersonAsync(
            AppDbContext context,
            BookingRepository bookingRepo,
            IReadOnlyList<BookingCustomer> allLinks,
            int idPerson,
            Booking anchorBooking,
            int paidStatusId,
            IReadOnlyList<Ticket> allTickets,
            CancellationToken ct)
    {
        var flightRepo = new FlightRepository(context);
        var routeRepo = new RouteRepository(context);
        var airportRepo = new AirportRepository(context);

        bool HasTicket(int idB) => allTickets.Any(t => t.IdBooking == idB);

        var bookingIdsForPerson = allLinks
            .Where(l => l.IdPerson == idPerson)
            .Select(l => l.IdBooking)
            .Distinct()
            .ToList();

        var candidates = new List<Booking>();
        foreach (var bid in bookingIdsForPerson)
        {
            var b = await bookingRepo.GetByIdAsync(BookingId.Create(bid), ct);
            if (b is null || b.IdStatus != paidStatusId || !HasTicket(b.Id.Value))
                continue;
            if (b.SeatCount.Value != anchorBooking.SeatCount.Value)
                continue;
            if (b.CreationDate.Value != anchorBooking.CreationDate.Value)
                continue;
            candidates.Add(b);
        }

        candidates = candidates
            .GroupBy(b => b.Id.Value)
            .Select(g => g.First())
            .OrderBy(b => b.FlightDate.Value)
            .ToList();

        var result = new List<(Booking, Flight, string, string)>();
        foreach (var b in candidates)
        {
            var f = await new GetFlightByIdUseCase(flightRepo).ExecuteAsync(b.IdFlight, ct);
            var route = await new GetRouteByIdUseCase(routeRepo).ExecuteAsync(f.IdRoute, ct);
            var origin = await new GetAirportByIdUseCase(airportRepo).ExecuteAsync(route.OriginAirport, ct);
            var dest = await new GetAirportByIdUseCase(airportRepo).ExecuteAsync(route.DestinationAirport, ct);
            var rl = $"{origin.IATACode.Value} → {dest.IATACode.Value}";
            result.Add((b, f, rl, ""));
        }

        if (result.Count == 0)
            return new List<(Booking, Flight, string, string)>();

        if (result.Count == 1)
            return new List<(Booking, Flight, string, string)> { (result[0].Item1, result[0].Item2, result[0].Item3, "Solo ida / trayecto único") };

        var labeled = new List<(Booking b, Flight f, string route, string kind)>();
        for (var i = 0; i < result.Count; i++)
        {
            var row = result[i];
            var kind = i == 0 ? "Ida" : i == result.Count - 1 ? "Regreso" : $"Tramo {i + 1}";
            labeled.Add((row.Item1, row.Item2, row.Item3, kind));
        }

        return labeled;
    }

    private static async Task<int> ResolveStatusIdAsync(AppDbContext context, string entityType, string name, CancellationToken ct)
    {
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var m = statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, entityType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, name, StringComparison.OrdinalIgnoreCase));
        if (m is null)
            throw new InvalidOperationException($"No existe el estado «{name}» para «{entityType}».");
        return m.Id.Value;
    }

    // Nota: control de acceso por reserva y matching de apellido se movieron a ExamCheckInService.

}
