using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;
using Spectre.Console;
using CheckInRecord = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;
using TicketAgg = SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate.Ticket;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.UI;

public sealed class TicketMenu
{
    private const string BookingEntityType = "Booking";
    private const string BookingStatusConfirmed = "Confirmada";
    private const string BookingStatusPaid = "Pagada";
    private const string BookingStatusCanceled = "Cancelada";
    private const string CheckInEntityType = "CheckIn";
    private const string CheckInStatusCompleted = "Completado";

    /// <summary>Reserva seleccionada en el módulo admin (opción «Buscar reserva»); requerida para las demás acciones.</summary>
    private static int? _adminModuleBookingId;

    public async Task RunAsync(CancellationToken ct = default)
    {
        bool isAdmin = AppState.IdUserRole == 1;
        bool back = false;
        while (!back)
        {
            Console.Clear();
            if (isAdmin)
                AnsiConsole.Write(new Rule("[green]GESTIÓN DE TIQUETES Y CHECK-IN[/]").Centered());
            else
                AnsiConsole.Write(new Rule("[green]GESTIÓN DE TIQUETES Y CHECK-IN[/]").Centered());

            if (isAdmin)
            {
                if (_adminModuleBookingId is int sid)
                    AnsiConsole.MarkupLine($"[grey]Reserva en sesión: [bold]ID {sid}[/]. Usá la opción 1 para cambiarla.[/]\n");
                else
                    AnsiConsole.MarkupLine("[grey]No hay reserva en sesión: primero usá [bold]Buscar reserva[/] (opción 1).[/]\n");

                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(8)
                        .AddChoices(
                            "1. Buscar reserva",
                            "2. Gestionar tiquete",
                            "3. Gestionar equipaje",
                            "4. Gestión de check-in",
                            "5. Imprimir pase de abordar",
                            "0. Volver"));
                switch (option)
                {
                    case "1. Buscar reserva":
                        await AdminBuscarReservaAsync(ct);
                        break;
                    case "2. Gestionar tiquete":
                        await AdminGestionarTiqueteAsync(ct);
                        break;
                    case "3. Gestionar equipaje":
                        await AdminGestionarEquipajeAsync(ct);
                        break;
                    case "4. Gestión de check-in":
                        await AdminGestionCheckInAsync(ct);
                        break;
                    case "5. Imprimir pase de abordar":
                        await AdminImprimirPaseAbordarAsync(ct);
                        break;
                    case "0. Volver":
                        _adminModuleBookingId = null;
                        back = true;
                        break;
                }
            }
            else
            {
                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(5)
                        .AddChoices(
                            "1. Check-in (PNR + apellido)",
                            "2. Ver mis tiquetes",
                            "3. Ver mis check-ins",
                            "0. Volver"));
                switch (option)
                {
                    case "1. Check-in (PNR + apellido)":
                        await ClientPnrCheckInMenu.RunAsync(ct);
                        break;
                    case "2. Ver mis tiquetes":
                        await ListTicketsAsync(ct);
                        break;
                    case "3. Ver mis check-ins":
                        await ListCheckInsAsync(ct);
                        break;
                    case "0. Volver": back = true; break;
                }
            }
        }
    }

    private static async Task<HashSet<int>> GetMyBookingIdsAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
        var ids = new HashSet<int>();
        foreach (var l in links)
        {
            if (l.IdUser == AppState.IdUser)
                ids.Add(l.IdBooking);
            if (AppState.IdPerson is int myPersonId && l.IdPerson == myPersonId)
                ids.Add(l.IdBooking);
        }

        var all = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        foreach (var b in all)
        {
            if (ClientBookingCodeOwnership.CodeLooksLikeClientGeneratedBooking(b.Code.Value, AppState.IdUser))
                ids.Add(b.Id.Value);
        }

        return ids;
    }

    /// <summary>Reservas del usuario que además están en estado «Pagada» (tiquete emitido con pago registrado).</summary>
    private static async Task<HashSet<int>> GetMyPaidBookingIdsAsync(AppDbContext context, HashSet<int> myBookingIds, CancellationToken ct)
    {
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var paidStatus = statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, BookingEntityType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, BookingStatusPaid, StringComparison.OrdinalIgnoreCase));
        if (paidStatus is null)
            return new HashSet<int>();

        var allBookings = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        return allBookings
            .Where(b => b.IdStatus == paidStatus.Id.Value && myBookingIds.Contains(b.Id.Value))
            .Select(b => b.Id.Value)
            .ToHashSet();
    }

    private static async Task<HashSet<int>> GetMyTicketIdsAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var myBookingIds = await GetMyBookingIdsAsync(ct);
        var paidBookingIds = await GetMyPaidBookingIdsAsync(context, myBookingIds, ct);
        var tickets = await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct);
        return tickets
            .Where(t => paidBookingIds.Contains(t.IdBooking))
            .Select(t => t.Id.Value)
            .ToHashSet();
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

    /// <summary>Vista cliente: un panel por tiquete (sin tabla de IDs).</summary>
    private static async Task WriteClientTicketSummaryPanelsAsync(
        AppDbContext context,
        IReadOnlyList<TicketAgg> tickets,
        IReadOnlyDictionary<int, string> fareMap,
        IReadOnlyDictionary<int, string> statusMap,
        CancellationToken ct)
    {
        var bookingRepo = new BookingRepository(context);
        var flightRepo = new FlightRepository(context);
        var allBookings = await new GetAllBookingsUseCase(bookingRepo).ExecuteAsync(ct);
        var bookingById = allBookings.ToDictionary(b => b.Id.Value);
        var allFlights = await new GetAllFlightsUseCase(flightRepo).ExecuteAsync(ct);
        var flightById = allFlights.ToDictionary(f => f.Id.Value);

        foreach (var t in tickets.OrderByDescending(x => x.IssueDate.Value))
        {
            if (!bookingById.TryGetValue(t.IdBooking, out var bk))
                continue;
            if (!flightById.TryGetValue(bk.IdFlight, out var fl))
                continue;

            var routeLabel = await BuildRouteLabelForFlightAsync(context, fl.IdRoute, ct);
            var fare = fareMap.TryGetValue(t.IdFare, out var fn) ? fn : t.IdFare.ToString();
            var status = statusMap.TryGetValue(t.IdStatus, out var sn) ? sn : t.IdStatus.ToString();

            var panel = new Panel(
                    $"[bold]Código de tiquete:[/] {Markup.Escape(t.Code.Value)}\n" +
                    $"[bold]Reserva (PNR):[/] {Markup.Escape(bk.Code.Value)}\n" +
                    $"[bold]Vuelo:[/] {Markup.Escape(fl.Number.Value)}\n" +
                    $"[bold]Ruta:[/] {Markup.Escape(routeLabel)}\n" +
                    $"[bold]Fecha y hora del vuelo:[/] {fl.Date.Value:yyyy-MM-dd} {fl.DepartureTime.Value:hh\\:mm}\n" +
                    $"[bold]Emitido:[/] {t.IssueDate.Value:yyyy-MM-dd HH:mm}\n" +
                    $"[bold]Tarifa:[/] {Markup.Escape(fare)}\n" +
                    $"[bold]Estado del tiquete:[/] {Markup.Escape(status)}")
                .Header("[cyan]Tu tiquete[/]")
                .Border(BoxBorder.Rounded);

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }
    }

    /// <summary>Vista cliente: un panel por check-in, estilo resumen de abordaje.</summary>
    private static async Task WriteClientCheckInSummaryPanelsAsync(
        AppDbContext context,
        IReadOnlyList<CheckInRecord> checkIns,
        IReadOnlyDictionary<int, string> channelMap,
        IReadOnlyDictionary<int, string> statusMap,
        CancellationToken ct)
    {
        var ticketRepo = new TicketRepository(context);
        var bookingRepo = new BookingRepository(context);
        var flightRepo = new FlightRepository(context);
        var seatRepo = new SeatRepository(context);

        var allTickets = await new GetAllTicketsUseCase(ticketRepo).ExecuteAsync(ct);
        var ticketById = allTickets.ToDictionary(x => x.Id.Value);
        var allBookings = await new GetAllBookingsUseCase(bookingRepo).ExecuteAsync(ct);
        var bookingById = allBookings.ToDictionary(b => b.Id.Value);
        var allFlights = await new GetAllFlightsUseCase(flightRepo).ExecuteAsync(ct);
        var flightById = allFlights.ToDictionary(f => f.Id.Value);
        var allSeats = await new GetAllSeatsUseCase(seatRepo).ExecuteAsync(ct);
        var seatById = allSeats.ToDictionary(s => s.Id.Value, s => s);

        var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
        var persons = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
        var personById = persons.ToDictionary(p => p.Id.Value);

        foreach (var c in checkIns.OrderByDescending(x => x.Date.Value))
        {
            if (!ticketById.TryGetValue(c.IdTicket, out var ticket))
                continue;
            if (!bookingById.TryGetValue(ticket.IdBooking, out var bk))
                continue;
            if (!flightById.TryGetValue(bk.IdFlight, out var fl))
                continue;

            var routeLabel = await BuildRouteLabelForFlightAsync(context, fl.IdRoute, ct);
            var seatLabel = seatById.TryGetValue(c.IdSeat, out var seat) ? seat.Number.Value : c.IdSeat.ToString();
            var link = links.FirstOrDefault(l => l.IdBooking == ticket.IdBooking && l.IdSeat == c.IdSeat);
            var passenger = link is not null && personById.TryGetValue(link.IdPerson, out var pers)
                ? $"{pers.FirstName.Value} {pers.LastName.Value}".Trim()
                : "—";
            var channel = channelMap.TryGetValue(c.IdChannel, out var cn) ? cn : c.IdChannel.ToString();
            var status = statusMap.TryGetValue(c.IdStatus, out var sn) ? sn : c.IdStatus.ToString();

            var panel = new Panel(
                    $"[bold]Pasajero:[/] {Markup.Escape(passenger)}\n" +
                    $"[bold]Tiquete:[/] {Markup.Escape(ticket.Code.Value)}\n" +
                    $"[bold]Reserva (PNR):[/] {Markup.Escape(bk.Code.Value)}\n" +
                    $"[bold]Vuelo:[/] {Markup.Escape(fl.Number.Value)}\n" +
                    $"[bold]Ruta:[/] {Markup.Escape(routeLabel)}\n" +
                    $"[bold]Salida del vuelo:[/] {fl.Date.Value:yyyy-MM-dd} {fl.DepartureTime.Value:hh\\:mm}\n" +
                    $"[bold]Asiento:[/] {Markup.Escape(seatLabel)}\n" +
                    $"[bold]Fecha del check-in:[/] {c.Date.Value:yyyy-MM-dd HH:mm}\n" +
                    $"[bold]Canal:[/] {Markup.Escape(channel)}\n" +
                    $"[bold]Estado:[/] {Markup.Escape(status)}")
                .Header("[green]Check-in registrado[/]")
                .Border(BoxBorder.Heavy);

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }
    }

    private static async Task<int> SelectStatusAsync(string entityType, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var filtered = statuses.Where(s => s.EntityType.Value == entityType).ToList();
        if (!filtered.Any()) throw new InvalidOperationException($"No hay estados para '{entityType}'. Crea uno en Administración.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el estado:").PageSize(10)
                .AddChoices(filtered.Select(s => $"{s.Id.Value}. {s.Name.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task ListTicketsAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var tickets = await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct);
        var fares = await new GetAllFaresUseCase(new FareRepository(context)).ExecuteAsync(ct);
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var fareMap = fares.ToDictionary(f => f.Id.Value, f => f.Name.Value);
        var statusMap = statuses.ToDictionary(s => s.Id.Value, s => s.Name.Value);

        if (AppState.IdUserRole != 1)
        {
            var myBookingIds = await GetMyBookingIdsAsync(ct);
            var paidBookingIds = await GetMyPaidBookingIdsAsync(context, myBookingIds, ct);
            tickets = tickets.Where(t => paidBookingIds.Contains(t.IdBooking)).ToList();
        }

        if (!tickets.Any())
        {
            AnsiConsole.MarkupLine(
                AppState.IdUserRole != 1
                    ? "[yellow]No tenés tiquetes en reservas pagadas todavía, o no hay coincidencias.[/]"
                    : "[yellow]No hay tiquetes registrados.[/]");
        }
        else if (AppState.IdUserRole != 1)
        {
            AnsiConsole.Write(new Rule("[green]MIS TIQUETES[/]").Centered());
            AnsiConsole.MarkupLine(
                "[grey]Solo se muestran tiquetes de reservas [bold]pagadas[/]. Cada uno aparece en un recuadro con vuelo, ruta y datos útiles.[/]\n");
            await WriteClientTicketSummaryPanelsAsync(context, tickets, fareMap, statusMap, ct);
        }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Código"); table.AddColumn("Emisión");
            table.AddColumn("Reserva ID"); table.AddColumn("Tarifa"); table.AddColumn("Estado");
            foreach (var t in tickets)
            {
                var fare = fareMap.TryGetValue(t.IdFare, out var fn) ? fn : t.IdFare.ToString();
                var status = statusMap.TryGetValue(t.IdStatus, out var sn) ? sn : t.IdStatus.ToString();
                table.AddRow(t.Id.Value.ToString(), Markup.Escape(t.Code.Value),
                    t.IssueDate.Value.ToString("yyyy-MM-dd HH:mm"),
                    t.IdBooking.ToString(), Markup.Escape(fare), Markup.Escape(status));
            }
            AnsiConsole.Write(table);
        }

        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task<int> SelectFareAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var fares = await new GetAllFaresUseCase(new FareRepository(context)).ExecuteAsync(ct);
        var active = fares.Where(f => f.Active).ToList();
        if (!active.Any()) throw new InvalidOperationException("No hay tarifas activas. Crea una primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona la tarifa:").PageSize(10)
                .AddChoices(active.Select(f => $"{f.Id.Value}. {f.Name.Value} (${f.BasePrice.Value:N2})")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task CreateTicketAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]EMITIR TIQUETE[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas emitir un tiquete?", true))
            return;
        var code = AnsiConsole.Prompt(
            new TextPrompt<string>("Código del tiquete (5-20 caracteres, solo letras y números, sin guiones. Ej: TK009):")
                .Validate(v =>
                {
                    try
                    {
                        _ = TicketCode.Create(v);
                        return ValidationResult.Success();
                    }
                    catch (ArgumentException ex)
                    {
                        return ValidationResult.Error($"[red]{Markup.Escape(ex.Message)}[/]");
                    }
                }));
        var idBooking = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la reserva (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (idBooking == 0) return;
        try
        {
            using var preCtx = DbContextFactory.Create();
            var booking = await new GetBookingByIdUseCase(new BookingRepository(preCtx)).ExecuteAsync(idBooking, ct);
            var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(preCtx)).ExecuteAsync(ct);
            var confirmedId = statuses
                .Where(s => s.EntityType.Value == BookingEntityType && s.Name.Value == BookingStatusConfirmed)
                .Select(s => s.Id.Value)
                .FirstOrDefault();
            if (confirmedId <= 0)
                throw new InvalidOperationException("No existe el estado 'Confirmada' para Booking (Semillas).");
            if (booking.IdStatus != confirmedId)
                throw new InvalidOperationException("Solo se pueden emitir tiquetes desde reservas CONFIRMADAS.");

            var idFare = await SelectFareAsync(ct);
            var idStatus = await SelectStatusAsync("Ticket", ct);
            using var context = DbContextFactory.Create();
            var result = await new CreateTicketUseCase(new TicketRepository(context))
                .ExecuteAsync(code, idBooking, idFare, idStatus, ct);
            await context.SaveChangesAsync(ct);

            var ticketRepo = new TicketRepository(context);
            var saved = await ticketRepo.GetByCodeAsync(result.Code.Value, ct);
            if (saved is not null)
                AnsiConsole.MarkupLine($"\n[green]Tiquete '[bold]{Markup.Escape(saved.Code.Value)}[/]' emitido con ID {saved.Id.Value}.[/]");
            else
                AnsiConsole.MarkupLine($"\n[green]Tiquete '[bold]{Markup.Escape(result.Code.Value)}[/]' emitido correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task TicketStatusAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CAMBIO DE ESTADO DE TIQUETE[/]").Centered());
        var idTicket = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del tiquete (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (idTicket == 0) return;
        var obs = AnsiConsole.Prompt(
                new TextPrompt<string>("Observación (Enter para omitir):")
                    .AllowEmpty())
            .Trim();
        string? observation = string.IsNullOrEmpty(obs) ? null : obs;
        try
        {
            var idStatus = await SelectStatusAsync("Ticket", ct);
            using var context = DbContextFactory.Create();
            await new CreateTicketStatusHistoryUseCase(new TicketStatusHistoryRepository(context))
                .ExecuteAsync(DateTime.Now, observation, idTicket, idStatus, AppState.IdUser, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Cambio de estado registrado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task DeleteTicketAsync(CancellationToken ct)
    {
        Console.Clear();
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del tiquete a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el tiquete con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteTicketUseCase(new TicketRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Tiquete eliminado correctamente.[/]" : "\n[yellow]No se encontró el tiquete.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task ListBaggageAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var baggages = await new GetAllBaggagesUseCase(new BaggageRepository(context)).ExecuteAsync(ct);
        var types = await new GetAllBaggageTypesUseCase(new BaggageTypeRepository(context)).ExecuteAsync(ct);
        var typeMap = types.ToDictionary(t => t.Id.Value, t => t.Name.Value);

        if (!baggages.Any()) { AnsiConsole.MarkupLine("[yellow]No hay equipaje registrado.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Tiquete ID"); table.AddColumn("Peso (kg)"); table.AddColumn("Tipo");
            foreach (var b in baggages)
            {
                var type = typeMap.TryGetValue(b.IdBaggageType, out var tn) ? tn : b.IdBaggageType.ToString();
                table.AddRow(b.Id.Value.ToString(), b.IdTicket.ToString(),
                    b.Weight.Value.ToString("F2"), Markup.Escape(type));
            }
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task<int> SelectBaggageTypeAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllBaggageTypesUseCase(new BaggageTypeRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay tipos de equipaje. Crea uno en Administración.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el tipo de equipaje:").PageSize(10)
                .AddChoices(items.Select(t => $"{t.Id.Value}. {t.Name.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task CreateBaggageAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]REGISTRAR EQUIPAJE[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas registrar equipaje?", true))
            return;
        var idTicket = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del tiquete (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (idTicket == 0) return;
        var weight = AnsiConsole.Prompt(
            new TextPrompt<decimal>(
                    $"Peso en kg (mayor que 0 y hasta {BaggageWeight.MaximumKilograms:0} kg por pieza; ej. cabina 8–12, bodega 18–25):")
                .Validate(v =>
                {
                    if (v <= 0)
                        return ValidationResult.Error("[red]El peso debe ser mayor que 0 kg.[/]");
                    if (v > BaggageWeight.MaximumKilograms)
                        return ValidationResult.Error(
                            $"[red]No puede superar {BaggageWeight.MaximumKilograms:0} kg por pieza. Referencia: cabina ~7–12 kg, bodega ~15–32 kg.[/]");
                    return ValidationResult.Success();
                }));
        try
        {
            if (AppState.IdUserRole != 1)
            {
                var myTicketIds = await GetMyTicketIdsAsync(ct);
                if (!myTicketIds.Contains(idTicket))
                    throw new InvalidOperationException("No puedes registrar equipaje para un tiquete que no te pertenece.");
            }

            var idType = await SelectBaggageTypeAsync(ct);
            using var context = DbContextFactory.Create();
            var result = await new CreateBaggageUseCase(new BaggageRepository(context))
                .ExecuteAsync(weight, idTicket, idType, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllBaggagesUseCase(new BaggageRepository(context)).ExecuteAsync(ct))
                .Where(b => b.IdTicket == idTicket && b.IdBaggageType == idType && b.Weight.Value == weight)
                .OrderByDescending(b => b.Id.Value)
                .Select(b => b.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Equipaje registrado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task DeleteBaggageAsync(CancellationToken ct)
    {
        Console.Clear();
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del equipaje a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el equipaje con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteBaggageUseCase(new BaggageRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Equipaje eliminado correctamente.[/]" : "\n[yellow]No se encontró el equipaje.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task ListCheckInsAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var checkIns = (await new GetAllCheckInsUseCase(new CheckInRepository(context)).ExecuteAsync(ct)).ToList();
        var channels = await new GetAllCheckInChannelsUseCase(new CheckInChannelRepository(context)).ExecuteAsync(ct);
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var channelMap = channels.ToDictionary(c => c.Id.Value, c => c.Name.Value);
        var statusMap = statuses.ToDictionary(s => s.Id.Value, s => s.Name.Value);

        if (AppState.IdUserRole != 1)
        {
            var myTicketIds = await GetMyTicketIdsAsync(ct);
            checkIns = checkIns.Where(c => myTicketIds.Contains(c.IdTicket)).ToList();
        }

        if (!checkIns.Any())
        {
            AnsiConsole.MarkupLine(
                AppState.IdUserRole != 1
                    ? "[yellow]No hay check-ins en tiquetes de reservas pagadas, o todavía no registraste ninguno.[/]"
                    : "[yellow]No hay check-ins registrados.[/]");
        }
        else if (AppState.IdUserRole != 1)
        {
            AnsiConsole.Write(new Rule("[green]MIS CHECK-INS[/]").Centered());
            AnsiConsole.MarkupLine(
                "[grey]Solo check-ins de tiquetes en reservas [bold]pagadas[/]. Mismo estilo de resumen que al confirmar: vuelo, asiento y pasajero.[/]\n");
            await WriteClientCheckInSummaryPanelsAsync(context, checkIns, channelMap, statusMap, ct);
        }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Fecha"); table.AddColumn("Tiquete ID");
            table.AddColumn("Asiento ID"); table.AddColumn("Canal"); table.AddColumn("Estado");
            foreach (var c in checkIns)
            {
                var channel = channelMap.TryGetValue(c.IdChannel, out var cn) ? cn : c.IdChannel.ToString();
                var status = statusMap.TryGetValue(c.IdStatus, out var sn) ? sn : c.IdStatus.ToString();
                table.AddRow(c.Id.Value.ToString(), c.Date.Value.ToString("yyyy-MM-dd HH:mm"),
                    c.IdTicket.ToString(), c.IdSeat.ToString(),
                    Markup.Escape(channel), Markup.Escape(status));
            }
            AnsiConsole.Write(table);
        }

        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static bool AdminTryRequireBookingInSession(string accion)
    {
        if (_adminModuleBookingId is int)
            return true;
        AnsiConsole.MarkupLine(
            $"[yellow]Primero buscá una reserva (menú principal, opción 1) antes de «{Markup.Escape(accion)}».[/]");
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
        return false;
    }

    private static async Task<int> AdminResolveStatusIdAsync(AppDbContext context, string entityType, string name, CancellationToken ct)
    {
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var m = statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, entityType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, name, StringComparison.OrdinalIgnoreCase));
        if (m is null)
            throw new InvalidOperationException($"No existe el estado «{name}» para «{entityType}».");
        return m.Id.Value;
    }

    private static async Task AdminBuscarReservaAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[cyan]BUSCAR RESERVA[/]").Centered());
        var modo = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Criterio:")
                .PageSize(5)
                .AddChoices("PNR", "Documento", "Nombre y/o apellido", "0. Volver"));
        if (modo == "0. Volver")
            return;

        try
        {
            using var context = DbContextFactory.Create();
            var bookingRepo = new BookingRepository(context);
            var allBookings = await new GetAllBookingsUseCase(bookingRepo).ExecuteAsync(ct);

            if (modo == "PNR")
            {
                var raw = AnsiConsole.Prompt(
                    new TextPrompt<string>("Código PNR:")
                        .Validate(s =>
                            string.IsNullOrWhiteSpace(s)
                                ? ValidationResult.Error("[red]Ingresá un código.[/]")
                                : ValidationResult.Success()));
                var code = raw.Trim().ToUpperInvariant();
                var b = await bookingRepo.GetByCodeAsync(code, ct);
                if (b is null)
                {
                    AnsiConsole.MarkupLine("[red]No se encontró una reserva con ese PNR.[/]");
                }
                else
                {
                    _adminModuleBookingId = b.Id.Value;
                    AnsiConsole.MarkupLine($"[green]Reserva cargada en sesión: PNR [bold]{Markup.Escape(b.Code.Value)}[/] (ID {b.Id.Value}).[/]");
                }
            }
            else if (modo == "Documento")
            {
                var docInput = AnsiConsole.Prompt(
                    new TextPrompt<string>("Número de documento:")
                        .Validate(s =>
                            string.IsNullOrWhiteSpace(s)
                                ? ValidationResult.Error("[red]Ingresá un documento.[/]")
                                : ValidationResult.Success()));
                var normalized = PersonDocumentNumber.Create(docInput.Trim()).Value;
                var personRepo = new PersonRepository(context);
                var person = await personRepo.GetByDocumentNumberAsync(normalized, ct);
                if (person is null)
                {
                    AnsiConsole.MarkupLine("[red]No hay persona registrada con ese documento.[/]");
                }
                else
                {
                    var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
                    var ids = links.Where(l => l.IdPerson == person.Id.Value).Select(l => l.IdBooking).Distinct().ToList();
                    var matches = allBookings.Where(b => ids.Contains(b.Id.Value)).OrderBy(b => b.Code.Value).ToList();
                    if (matches.Count == 0)
                        AnsiConsole.MarkupLine("[red]Esa persona no figura en ninguna reserva.[/]");
                    else
                        _adminModuleBookingId = (await AdminPickBookingFromListAsync(matches, ct))?.Id.Value;
                    if (_adminModuleBookingId is int bid)
                        AnsiConsole.MarkupLine($"[green]Reserva cargada en sesión (ID {bid}).[/]");
                }
            }
            else
            {
                var nombre = AnsiConsole.Prompt(
                    new TextPrompt<string>("Nombre (opcional, Enter para omitir):").AllowEmpty());
                var apellido = AnsiConsole.Prompt(
                    new TextPrompt<string>("Apellido (opcional, Enter para omitir):").AllowEmpty());
                var fn = (nombre ?? string.Empty).Trim();
                var ln = (apellido ?? string.Empty).Trim();
                if (fn.Length == 0 && ln.Length == 0)
                {
                    AnsiConsole.MarkupLine("[red]Ingresá al menos nombre o apellido.[/]");
                }
                else
                {
                    var persons = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
                    var hitIds = persons
                        .Where(p =>
                            (fn.Length == 0 || p.FirstName.Value.Contains(fn, StringComparison.OrdinalIgnoreCase)) &&
                            (ln.Length == 0 || p.LastName.Value.Contains(ln, StringComparison.OrdinalIgnoreCase)))
                        .Select(p => p.Id.Value)
                        .ToHashSet();
                    if (hitIds.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]No hay personas que coincidan con ese criterio.[/]");
                    }
                    else
                    {
                        var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
                        var bookingIds = links.Where(l => hitIds.Contains(l.IdPerson)).Select(l => l.IdBooking).Distinct().ToList();
                        var matches = allBookings.Where(b => bookingIds.Contains(b.Id.Value)).OrderBy(b => b.Code.Value).ToList();
                        if (matches.Count == 0)
                            AnsiConsole.MarkupLine("[red]No hay reservas asociadas a esas personas.[/]");
                        else
                            _adminModuleBookingId = (await AdminPickBookingFromListAsync(matches, ct))?.Id.Value;
                        if (_adminModuleBookingId is int bid)
                            AnsiConsole.MarkupLine($"[green]Reserva cargada en sesión (ID {bid}).[/]");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task<Booking?> AdminPickBookingFromListAsync(IReadOnlyList<Booking> matches, CancellationToken ct)
    {
        if (matches.Count == 1)
            return matches[0];
        var pick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Varias reservas coinciden. Elegí una:")
                .PageSize(12)
                .AddChoices(matches.Select(b => $"{b.Id.Value} — PNR {b.Code.Value} — Vuelo {b.IdFlight}")));
        var dash = pick.IndexOf('—', StringComparison.Ordinal);
        var idPart = dash > 0 ? pick.AsSpan(0, dash).Trim() : pick.AsSpan(0, pick.IndexOf(' '));
        var id = int.Parse(idPart.ToString(), System.Globalization.CultureInfo.InvariantCulture);
        return matches.FirstOrDefault(b => b.Id.Value == id);
    }

    private static async Task AdminGestionarTiqueteAsync(CancellationToken ct)
    {
        if (!AdminTryRequireBookingInSession("Gestionar tiquete"))
            return;

        bool subBack = false;
        while (!subBack)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[cyan]GESTIONAR TIQUETE[/]").Centered());
            AnsiConsole.MarkupLine($"[grey]Reserva en sesión: ID [bold]{_adminModuleBookingId}[/][/]\n");
            var sub = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices(
                        "1. Ver información completa",
                        "2. Modificar datos del tiquete",
                        "3. Cancelar reserva",
                        "4. Cambiar estado del tiquete",
                        "0. Volver"));
            switch (sub)
            {
                case "1. Ver información completa":
                    await AdminVerInformacionCompletaReservaAsync(ct);
                    break;
                case "2. Modificar datos del tiquete":
                    await AdminModificarTiqueteSesionAsync(ct);
                    break;
                case "3. Cancelar reserva":
                    await AdminCancelarReservaSesionAsync(ct);
                    break;
                case "4. Cambiar estado del tiquete":
                    await AdminCambiarEstadoTiqueteSesionAsync(ct);
                    break;
                case "0. Volver":
                    subBack = true;
                    break;
            }
        }
    }

    private static async Task AdminVerInformacionCompletaReservaAsync(CancellationToken ct)
    {
        Console.Clear();
        try
        {
            using var context = DbContextFactory.Create();
            var idBooking = _adminModuleBookingId!.Value;
            var booking = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(idBooking, ct);
            if (booking is null)
            {
                AnsiConsole.MarkupLine("[red]La reserva de la sesión ya no existe. Buscá de nuevo.[/]");
                _adminModuleBookingId = null;
            }
            else
            {
                var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
                var stName = statuses.FirstOrDefault(s => s.Id.Value == booking.IdStatus)?.Name.Value ?? booking.IdStatus.ToString();
                var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(booking.IdFlight, ct);
                var routeLabel = await BuildRouteLabelForFlightAsync(context, flight.IdRoute, ct);
                var tickets = (await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct))
                    .Where(t => t.IdBooking == idBooking)
                    .OrderBy(t => t.Id.Value)
                    .ToList();
                var fares = await new GetAllFaresUseCase(new FareRepository(context)).ExecuteAsync(ct);
                var fareMap = fares.ToDictionary(f => f.Id.Value, f => f.Name.Value);
                var ticketStatusMap = statuses.Where(s => s.EntityType.Value == "Ticket").ToDictionary(s => s.Id.Value, s => s.Name.Value);

                var links = (await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct))
                    .Where(l => l.IdBooking == idBooking)
                    .ToList();
                var persons = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
                var personById = persons.ToDictionary(p => p.Id.Value);
                var seats = await new GetAllSeatsUseCase(new SeatRepository(context)).ExecuteAsync(ct);
                var seatById = seats.ToDictionary(s => s.Id.Value);

                var pax = new List<string>();
                foreach (var l in links.OrderByDescending(x => x.IsPrimary))
                {
                    if (!personById.TryGetValue(l.IdPerson, out var pers))
                        continue;
                    var seatLbl = l.IdSeat > 0 && seatById.TryGetValue(l.IdSeat, out var se)
                        ? se.Number.Value
                        : "(sin asiento)";
                    var prim = l.IsPrimary ? " [bold](titular)[/]" : "";
                    pax.Add(
                        $"  • {Markup.Escape(pers.FirstName.Value)} {Markup.Escape(pers.LastName.Value)} — doc. {Markup.Escape(pers.DocumentNumber.Value)} — asiento {Markup.Escape(seatLbl)}{prim}");
                }

                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"[bold]PNR:[/] {Markup.Escape(booking.Code.Value)}");
                sb.AppendLine($"[bold]ID reserva:[/] {booking.Id.Value}");
                sb.AppendLine($"[bold]Estado reserva:[/] {Markup.Escape(stName)}");
                sb.AppendLine($"[bold]Vuelo:[/] {Markup.Escape(flight.Number.Value)} · [bold]Ruta:[/] {Markup.Escape(routeLabel)}");
                sb.AppendLine($"[bold]Fecha vuelo (reserva):[/] {booking.FlightDate.Value:yyyy-MM-dd}");
                sb.AppendLine($"[bold]Salida programada:[/] {flight.Date.Value:yyyy-MM-dd} {flight.DepartureTime.Value:hh\\:mm}");
                sb.AppendLine($"[bold]Plazas:[/] {booking.SeatCount.Value}");
                sb.AppendLine($"[bold]Observaciones:[/] {Markup.Escape(booking.Observations.Value ?? "—")}");
                sb.AppendLine();
                sb.AppendLine("[bold]Pasajeros:[/]");
                sb.AppendLine(pax.Count > 0 ? string.Join('\n', pax) : "  (ninguno)");
                sb.AppendLine();
                sb.AppendLine("[bold]Tiquetes:[/]");
                if (tickets.Count == 0)
                    sb.AppendLine("  (ninguno emitido)");
                else
                {
                    foreach (var t in tickets)
                    {
                        var fare = fareMap.TryGetValue(t.IdFare, out var fn) ? fn : t.IdFare.ToString();
                        var ts = ticketStatusMap.TryGetValue(t.IdStatus, out var tn) ? tn : t.IdStatus.ToString();
                        sb.AppendLine(
                            $"  • ID {t.Id.Value} — {Markup.Escape(t.Code.Value)} — tarifa {Markup.Escape(fare)} — estado {Markup.Escape(ts)} — emitido {t.IssueDate.Value:yyyy-MM-dd HH:mm}");
                    }
                }

                AnsiConsole.Write(new Panel(sb.ToString()).Header("[green]Reserva y tiquetes[/]").Border(BoxBorder.Rounded));
            }
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task AdminModificarTiqueteSesionAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]MODIFICAR TIQUETE[/]").Centered());
        try
        {
            using var context = DbContextFactory.Create();
            var idBooking = _adminModuleBookingId!.Value;
            var tickets = (await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct))
                .Where(t => t.IdBooking == idBooking)
                .OrderBy(t => t.Id.Value)
                .ToList();
            if (tickets.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay tiquetes emitidos para esta reserva.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Tiquete a modificar:")
                    .PageSize(8)
                    .AddChoices(tickets.Select(t => $"{t.Id.Value}. {t.Code.Value}")));
            var idTicket = int.Parse(pick.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);
            var ticket = tickets.First(t => t.Id.Value == idTicket);

            var code = AnsiConsole.Prompt(
                new TextPrompt<string>("Código del tiquete:")
                    .DefaultValue(ticket.Code.Value)
                    .Validate(v =>
                    {
                        try
                        {
                            _ = TicketCode.Create(v);
                            return ValidationResult.Success();
                        }
                        catch (ArgumentException ex)
                        {
                            return ValidationResult.Error($"[red]{Markup.Escape(ex.Message)}[/]");
                        }
                    }));
            var issueDate = AnsiConsole.Prompt(
                new TextPrompt<string>($"Fecha y hora de emisión (yyyy-MM-dd HH:mm), actual {ticket.IssueDate.Value:yyyy-MM-dd HH:mm}:")
                    .Validate(s =>
                    {
                        if (!DateTime.TryParse(s, out _))
                            return ValidationResult.Error("[red]Formato inválido.[/]");
                        return ValidationResult.Success();
                    }));
            var parsedIssue = DateTime.Parse(issueDate, System.Globalization.CultureInfo.InvariantCulture);
            var idFare = await SelectFareAsync(ct);
            var idStatus = await SelectStatusAsync("Ticket", ct);

            await new UpdateTicketUseCase(new TicketRepository(context))
                .ExecuteAsync(idTicket, code, parsedIssue, idBooking, idFare, idStatus, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("[green]Tiquete actualizado.[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task AdminCancelarReservaSesionAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[red]CANCELAR RESERVA[/]").Centered());
        if (!AnsiConsole.Confirm("¿Confirmás cancelar la reserva en sesión y liberar cupos?", false))
            return;

        const string defaultCancelReason = "Cancelación administrativa (módulo tiquetes)";
        var reasonInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Motivo (Enter = texto por defecto):")
                .AllowEmpty());
        var reason = string.IsNullOrWhiteSpace(reasonInput) ? defaultCancelReason : reasonInput.Trim();
        if (reason.Length < 5)
            reason = defaultCancelReason;
        var penalty = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Penalización (0 si no aplica):")
                .DefaultValue(0m));

        try
        {
            using var context = DbContextFactory.Create();
            var bookingRepo = new BookingRepository(context);
            var idBooking = _adminModuleBookingId!.Value;
            var booking = await new GetBookingByIdUseCase(bookingRepo).ExecuteAsync(idBooking, ct)
                ?? throw new InvalidOperationException("No existe la reserva.");
            var canceledStatusId = await AdminResolveStatusIdAsync(context, BookingEntityType, BookingStatusCanceled, ct);
            if (booking.IdStatus == canceledStatusId)
            {
                AnsiConsole.MarkupLine("[yellow]La reserva ya está cancelada.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(booking.IdFlight, ct);

            await new CreateBookingCancellationUseCase(new BookingCancellationRepository(context))
                .ExecuteAsync(DateTime.Now, reason, penalty, idBooking, AppState.IdUser, ct);

            await new UpdateBookingUseCase(bookingRepo)
                .ExecuteAsync(
                    booking.Id.Value,
                    booking.Code.Value,
                    booking.FlightDate.Value,
                    booking.CreationDate.Value,
                    booking.SeatCount.Value,
                    booking.Observations.Value,
                    booking.IdFlight,
                    canceledStatusId,
                    ct);

            await new UpdateFlightUseCase(new FlightRepository(context))
                .ExecuteAsync(
                    flight.Id.Value,
                    flight.Number.Value,
                    flight.Date.Value,
                    flight.DepartureTime.Value,
                    flight.ArrivalTime.Value,
                    flight.TotalCapacity.Value,
                    flight.AvailableSeats.Value + booking.SeatCount.Value,
                    flight.IdRoute,
                    flight.IdAircraft,
                    flight.IdStatus,
                    flight.IdCrew,
                    flight.IdFare,
                    ct);

            await AdminReleaseSeatFlightsForBookingAsync(context, booking.Id.Value, booking.IdFlight, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("[green]Reserva cancelada y cupos actualizados.[/]");
            _adminModuleBookingId = null;
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task AdminReleaseSeatFlightsForBookingAsync(AppDbContext context, int idBooking, int idFlight, CancellationToken ct)
    {
        var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
        var seatIds = links.Where(l => l.IdBooking == idBooking).Select(l => l.IdSeat).Where(id => id > 0).Distinct().ToList();
        foreach (var idSeat in seatIds)
            await AdminSetSeatFlightAvailabilityAsync(context, idSeat, idFlight, available: true, ct);
    }

    private static async Task AdminSetSeatFlightAvailabilityAsync(AppDbContext context, int idSeat, int idFlight, bool available, CancellationToken ct)
    {
        var repo = new SeatFlightRepository(context);
        var seatFlight = await repo.GetBySeatAndFlightAsync(idSeat, idFlight, ct);
        if (seatFlight is null)
            return;
        if (seatFlight.Available == available)
            return;
        await new UpdateSeatFlightUseCase(repo).ExecuteAsync(seatFlight.Id.Value, seatFlight.IdSeat, seatFlight.IdFlight, available, ct);
    }

    private static async Task AdminCambiarEstadoTiqueteSesionAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CAMBIO DE ESTADO — TIQUETE[/]").Centered());
        try
        {
            using var context = DbContextFactory.Create();
            var idBooking = _adminModuleBookingId!.Value;
            var tickets = (await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct))
                .Where(t => t.IdBooking == idBooking)
                .OrderBy(t => t.Id.Value)
                .ToList();
            if (tickets.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay tiquetes para esta reserva.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Tiquete:")
                    .PageSize(8)
                    .AddChoices(tickets.Select(t => $"{t.Id.Value}. {t.Code.Value}")));
            var idTicket = int.Parse(pick.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);
            var obs = AnsiConsole.Prompt(
                    new TextPrompt<string>("Observación (Enter para omitir):")
                        .AllowEmpty())
                .Trim();
            string? observation = string.IsNullOrEmpty(obs) ? null : obs;
            var idStatus = await SelectStatusAsync("Ticket", ct);
            await new CreateTicketStatusHistoryUseCase(new TicketStatusHistoryRepository(context))
                .ExecuteAsync(DateTime.Now, observation, idTicket, idStatus, AppState.IdUser, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("[green]Historial de estado registrado.[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task AdminGestionarEquipajeAsync(CancellationToken ct)
    {
        if (!AdminTryRequireBookingInSession("Gestionar equipaje"))
            return;

        bool subBack = false;
        while (!subBack)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[cyan]GESTIONAR EQUIPAJE[/]").Centered());
            AnsiConsole.MarkupLine($"[grey]Reserva en sesión: ID [bold]{_adminModuleBookingId}[/][/]\n");
            var sub = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(5)
                    .AddChoices(
                        "1. Registrar equipaje",
                        "2. Modificar equipaje",
                        "3. Consultar equipaje",
                        "0. Volver"));
            switch (sub)
            {
                case "1. Registrar equipaje":
                    await AdminRegistrarEquipajeSesionAsync(ct);
                    break;
                case "2. Modificar equipaje":
                    await AdminModificarEquipajeSesionAsync(ct);
                    break;
                case "3. Consultar equipaje":
                    await AdminConsultarEquipajeSesionAsync(ct);
                    break;
                case "0. Volver":
                    subBack = true;
                    break;
            }
        }
    }

    private static async Task<IReadOnlyList<TicketAgg>> AdminGetTicketsForSessionAsync(AppDbContext context, CancellationToken ct)
    {
        var idBooking = _adminModuleBookingId!.Value;
        return (await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct))
            .Where(t => t.IdBooking == idBooking)
            .OrderBy(t => t.Id.Value)
            .ToList();
    }

    private static async Task AdminRegistrarEquipajeSesionAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]REGISTRAR EQUIPAJE[/]").Centered());
        if (!AnsiConsole.Confirm("¿Registrar equipaje para un tiquete de esta reserva?", true))
            return;
        try
        {
            using var context = DbContextFactory.Create();
            var tickets = await AdminGetTicketsForSessionAsync(context, ct);
            if (tickets.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay tiquetes emitidos; no se puede registrar equipaje.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Tiquete:")
                    .PageSize(8)
                    .AddChoices(tickets.Select(t => $"{t.Id.Value}. {t.Code.Value}")));
            var idTicket = int.Parse(pick.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);
            var weight = AnsiConsole.Prompt(
                new TextPrompt<decimal>(
                        $"Peso en kg (máx. {BaggageWeight.MaximumKilograms:0}):")
                    .Validate(v =>
                    {
                        if (v <= 0)
                            return ValidationResult.Error("[red]Debe ser mayor a 0.[/]");
                        if (v > BaggageWeight.MaximumKilograms)
                            return ValidationResult.Error($"[red]Máximo {BaggageWeight.MaximumKilograms:0} kg.[/]");
                        return ValidationResult.Success();
                    }));
            var idType = await SelectBaggageTypeAsync(ct);
            await new CreateBaggageUseCase(new BaggageRepository(context))
                .ExecuteAsync(weight, idTicket, idType, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("[green]Equipaje registrado.[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task AdminModificarEquipajeSesionAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]MODIFICAR EQUIPAJE[/]").Centered());
        try
        {
            using var context = DbContextFactory.Create();
            var tickets = await AdminGetTicketsForSessionAsync(context, ct);
            var ticketIds = tickets.Select(t => t.Id.Value).ToHashSet();
            if (ticketIds.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay tiquetes en esta reserva.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            var baggages = (await new GetAllBaggagesUseCase(new BaggageRepository(context)).ExecuteAsync(ct))
                .Where(b => ticketIds.Contains(b.IdTicket))
                .OrderBy(b => b.Id.Value)
                .ToList();
            if (baggages.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay equipaje registrado para los tiquetes de esta reserva.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Pieza de equipaje:")
                    .PageSize(10)
                    .AddChoices(baggages.Select(b => $"{b.Id.Value}. Tiquete {b.IdTicket} — {b.Weight.Value:F1} kg")));
            var idBag = int.Parse(pick.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);
            var bag = baggages.First(b => b.Id.Value == idBag);
            var weight = AnsiConsole.Prompt(
                new TextPrompt<decimal>($"Nuevo peso (kg), actual {bag.Weight.Value:F1}:")
                    .Validate(v =>
                    {
                        if (v <= 0 || v > BaggageWeight.MaximumKilograms)
                            return ValidationResult.Error("[red]Peso inválido.[/]");
                        return ValidationResult.Success();
                    }));
            var idType = await SelectBaggageTypeAsync(ct);
            await new UpdateBaggageUseCase(new BaggageRepository(context))
                .ExecuteAsync(idBag, weight, bag.IdTicket, idType, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("[green]Equipaje actualizado.[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task AdminConsultarEquipajeSesionAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]EQUIPAJE — RESERVA EN SESIÓN[/]").Centered());
        try
        {
            using var context = DbContextFactory.Create();
            var tickets = await AdminGetTicketsForSessionAsync(context, ct);
            var ticketIds = tickets.Select(t => t.Id.Value).ToHashSet();
            var types = await new GetAllBaggageTypesUseCase(new BaggageTypeRepository(context)).ExecuteAsync(ct);
            var typeMap = types.ToDictionary(t => t.Id.Value, t => t.Name.Value);
            var baggages = (await new GetAllBaggagesUseCase(new BaggageRepository(context)).ExecuteAsync(ct))
                .Where(b => ticketIds.Contains(b.IdTicket))
                .ToList();
            if (!baggages.Any())
                AnsiConsole.MarkupLine("[yellow]No hay equipaje para esta reserva.[/]");
            else
            {
                var table = new Table().Border(TableBorder.Rounded);
                table.AddColumn("ID");
                table.AddColumn("Tiquete");
                table.AddColumn("Peso");
                table.AddColumn("Tipo");
                foreach (var b in baggages.OrderBy(x => x.Id.Value))
                {
                    var tn = typeMap.TryGetValue(b.IdBaggageType, out var n) ? n : b.IdBaggageType.ToString();
                    table.AddRow(
                        b.Id.Value.ToString(),
                        b.IdTicket.ToString(),
                        b.Weight.Value.ToString("F2"),
                        Markup.Escape(tn));
                }

                AnsiConsole.Write(table);
            }
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task AdminGestionCheckInAsync(CancellationToken ct)
    {
        if (!AdminTryRequireBookingInSession("Gestión de check-in"))
            return;

        bool subBack = false;
        while (!subBack)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[cyan]GESTIÓN DE CHECK-IN[/]").Centered());
            AnsiConsole.MarkupLine($"[grey]Reserva en sesión: ID [bold]{_adminModuleBookingId}[/][/]\n");
            var sub = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(5)
                    .AddChoices(
                        "1. Realizar check-in manual",
                        "2. Ver check-ins",
                        "3. Revertir check-in",
                        "0. Volver"));
            switch (sub)
            {
                case "1. Realizar check-in manual":
                    await AdminCheckInManualSesionAsync(ct);
                    break;
                case "2. Ver check-ins":
                    await AdminListarCheckInsSesionAsync(ct);
                    break;
                case "3. Revertir check-in":
                    await AdminRevertirCheckInSesionAsync(ct);
                    break;
                case "0. Volver":
                    subBack = true;
                    break;
            }
        }
    }

    private static async Task AdminCheckInManualSesionAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CHECK-IN MANUAL[/]").Centered());
        if (!AnsiConsole.Confirm("¿Registrar check-in para esta reserva?", true))
            return;
        try
        {
            using var context = DbContextFactory.Create();
            var idBooking = _adminModuleBookingId!.Value;
            var paidId = await AdminResolveStatusIdAsync(context, BookingEntityType, BookingStatusPaid, ct);
            var booking = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(idBooking, ct)
                ?? throw new InvalidOperationException("No existe la reserva.");
            if (booking.IdStatus != paidId)
                throw new InvalidOperationException("No se puede hacer check-in: la reserva no está pagada.");

            var tickets = await AdminGetTicketsForSessionAsync(context, ct);
            if (tickets.Count == 0)
                throw new InvalidOperationException("No hay tiquete emitido para esta reserva.");

            var pickT = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Tiquete:")
                    .PageSize(8)
                    .AddChoices(tickets.Select(t => $"{t.Id.Value}. {t.Code.Value}")));
            var idTicket = int.Parse(pickT.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);
            var ticket = tickets.First(t => t.Id.Value == idTicket);

            var allCheckIns = await new GetAllCheckInsUseCase(new CheckInRepository(context)).ExecuteAsync(ct);
            if (allCheckIns.Any(c => c.IdTicket == idTicket))
                throw new InvalidOperationException("Este tiquete ya tiene check-in registrado.");

            var links = (await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct))
                .Where(l => l.IdBooking == idBooking && l.IdSeat > 0)
                .ToList();
            if (links.Count == 0)
                throw new InvalidOperationException("No hay asiento asignado en la reserva; asigná asientos antes del check-in.");

            BookingCustomer chosenLink;
            if (links.Count == 1)
                chosenLink = links[0];
            else
            {
                var persons = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
                var personById = persons.ToDictionary(p => p.Id.Value);
                var seats = await new GetAllSeatsUseCase(new SeatRepository(context)).ExecuteAsync(ct);
                var seatById = seats.ToDictionary(s => s.Id.Value);
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Pasajero / asiento para el check-in:")
                        .PageSize(10)
                        .AddChoices(links.Select(l =>
                        {
                            var p = personById.GetValueOrDefault(l.IdPerson);
                            var name = p is null ? $"Persona {l.IdPerson}" : $"{p.FirstName.Value} {p.LastName.Value}";
                            var sn = seatById.TryGetValue(l.IdSeat, out var se) ? se.Number.Value : l.IdSeat.ToString();
                            return $"{l.Id.Value} — {name} — asiento {sn}";
                        })));
                var linkId = int.Parse(choice.Split('—')[0].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                chosenLink = links.First(l => l.Id.Value == linkId);
            }

            var channels = await new GetAllCheckInChannelsUseCase(new CheckInChannelRepository(context)).ExecuteAsync(ct);
            if (channels.Count == 0)
                throw new InvalidOperationException("No hay canales de check-in configurados.");
            var selChannel = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Canal:")
                    .PageSize(10)
                    .AddChoices(channels.Select(c => $"{c.Id.Value}. {c.Name.Value}")));
            var idChannel = int.Parse(selChannel.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);

            var completedId = await AdminResolveStatusIdAsync(context, CheckInEntityType, CheckInStatusCompleted, ct);
            await new CreateCheckInUseCase(new CheckInRepository(context))
                .ExecuteAsync(DateTime.Now, idTicket, idChannel, chosenLink.IdSeat, AppState.IdUser, completedId, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("[green]Check-in registrado (estado Completado).[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task AdminListarCheckInsSesionAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]CHECK-INS — RESERVA EN SESIÓN[/]").Centered());
        try
        {
            using var context = DbContextFactory.Create();
            var tickets = await AdminGetTicketsForSessionAsync(context, ct);
            var ticketIds = tickets.Select(t => t.Id.Value).ToHashSet();
            var checkIns = (await new GetAllCheckInsUseCase(new CheckInRepository(context)).ExecuteAsync(ct))
                .Where(c => ticketIds.Contains(c.IdTicket))
                .OrderByDescending(c => c.Date.Value)
                .ToList();
            var channels = await new GetAllCheckInChannelsUseCase(new CheckInChannelRepository(context)).ExecuteAsync(ct);
            var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
            var channelMap = channels.ToDictionary(c => c.Id.Value, c => c.Name.Value);
            var statusMap = statuses.ToDictionary(s => s.Id.Value, s => s.Name.Value);
            if (!checkIns.Any())
                AnsiConsole.MarkupLine("[yellow]No hay check-ins para los tiquetes de esta reserva.[/]");
            else
            {
                var table = new Table().Border(TableBorder.Rounded);
                table.AddColumn("ID");
                table.AddColumn("Fecha");
                table.AddColumn("Tiquete");
                table.AddColumn("Asiento");
                table.AddColumn("Canal");
                table.AddColumn("Estado");
                var seats = await new GetAllSeatsUseCase(new SeatRepository(context)).ExecuteAsync(ct);
                var seatById = seats.ToDictionary(s => s.Id.Value);
                foreach (var c in checkIns)
                {
                    var seatLbl = seatById.TryGetValue(c.IdSeat, out var se) ? se.Number.Value : c.IdSeat.ToString();
                    table.AddRow(
                        c.Id.Value.ToString(),
                        c.Date.Value.ToString("yyyy-MM-dd HH:mm"),
                        c.IdTicket.ToString(),
                        Markup.Escape(seatLbl),
                        Markup.Escape(channelMap.TryGetValue(c.IdChannel, out var cn) ? cn : "?"),
                        Markup.Escape(statusMap.TryGetValue(c.IdStatus, out var sn) ? sn : "?"));
                }

                AnsiConsole.Write(table);
            }
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task AdminRevertirCheckInSesionAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[red]REVERTIR CHECK-IN[/]").Centered());
        try
        {
            using var context = DbContextFactory.Create();
            var tickets = await AdminGetTicketsForSessionAsync(context, ct);
            var ticketIds = tickets.Select(t => t.Id.Value).ToHashSet();
            var checkIns = (await new GetAllCheckInsUseCase(new CheckInRepository(context)).ExecuteAsync(ct))
                .Where(c => ticketIds.Contains(c.IdTicket))
                .OrderByDescending(c => c.Date.Value)
                .ToList();
            if (checkIns.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay check-ins para revertir.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Check-in a eliminar:")
                    .PageSize(10)
                    .AddChoices(checkIns.Select(c => $"{c.Id.Value}. Tiquete {c.IdTicket} — {c.Date.Value:yyyy-MM-dd HH:mm}")));
            var idCheckIn = int.Parse(pick.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);
            if (!AnsiConsole.Confirm("¿Eliminar ese registro de check-in?", false))
                return;
            var ok = await new DeleteCheckInUseCase(new CheckInRepository(context)).ExecuteAsync(idCheckIn, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(ok ? "[green]Check-in revertido.[/]" : "[yellow]No se encontró el check-in.[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task AdminImprimirPaseAbordarAsync(CancellationToken ct)
    {
        if (!AdminTryRequireBookingInSession("Imprimir pase de abordar"))
            return;

        Console.Clear();
        const string failMsg = "No se puede generar el pase de abordar. Verifique pago y check-in.";
        try
        {
            using var context = DbContextFactory.Create();
            var idBooking = _adminModuleBookingId!.Value;
            var paidId = await AdminResolveStatusIdAsync(context, BookingEntityType, BookingStatusPaid, ct);
            var completedId = await AdminResolveStatusIdAsync(context, CheckInEntityType, CheckInStatusCompleted, ct);
            var booking = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(idBooking, ct);
            if (booking is null || booking.IdStatus != paidId)
            {
                AnsiConsole.WriteLine(failMsg);
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            var tickets = await AdminGetTicketsForSessionAsync(context, ct);
            var ticketIds = tickets.Select(t => t.Id.Value).ToList();
            var checkIns = (await new GetAllCheckInsUseCase(new CheckInRepository(context)).ExecuteAsync(ct))
                .Where(c => ticketIds.Contains(c.IdTicket) && c.IdStatus == completedId && c.IdSeat > 0)
                .OrderByDescending(c => c.Date.Value)
                .ToList();
            if (checkIns.Count == 0)
            {
                AnsiConsole.WriteLine(failMsg);
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            CheckInRecord chosen = checkIns[0];
            if (checkIns.Count > 1)
            {
                var pick = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Varios check-ins completados. Elegí uno para el pase:")
                        .PageSize(8)
                        .AddChoices(checkIns.Select(c => $"{c.Id.Value}. Tiquete {c.IdTicket} — asiento {c.IdSeat}")));
                var idCh = int.Parse(pick.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);
                chosen = checkIns.First(c => c.Id.Value == idCh);
            }

            var ticket = tickets.First(t => t.Id.Value == chosen.IdTicket);
            var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(booking.IdFlight, ct);
            var routeLabel = await BuildRouteLabelForFlightAsync(context, flight.IdRoute, ct);
            var seats = await new GetAllSeatsUseCase(new SeatRepository(context)).ExecuteAsync(ct);
            var seatLabel = seats.FirstOrDefault(s => s.Id.Value == chosen.IdSeat)?.Number.Value ?? chosen.IdSeat.ToString();
            var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
            var link = links.FirstOrDefault(l => l.IdBooking == idBooking && l.IdSeat == chosen.IdSeat);
            var persons = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
            var passenger = "Pasajero";
            if (link is not null)
            {
                var pers = persons.FirstOrDefault(p => p.Id.Value == link.IdPerson);
                if (pers is not null)
                    passenger = $"{pers.FirstName.Value} {pers.LastName.Value}".Trim();
            }

            var gate = $"{(char)('A' + Random.Shared.Next(0, 3))}{Random.Shared.Next(1, 30):D2}";
            var group = Random.Shared.Next(1, 9);

            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("====================================");
            AnsiConsole.WriteLine("        PASE DE ABORDAR");
            AnsiConsole.WriteLine("====================================");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine($"Pasajero: {passenger}");
            AnsiConsole.WriteLine($"Código Reserva: {booking.Code.Value}");
            AnsiConsole.WriteLine($"Vuelo: {flight.Number.Value}");
            AnsiConsole.WriteLine($"Ruta: {routeLabel}");
            AnsiConsole.WriteLine($"Fecha: {flight.Date.Value:yyyy-MM-dd}");
            AnsiConsole.WriteLine($"Hora: {flight.DepartureTime.Value:hh\\:mm}");
            AnsiConsole.WriteLine($"Asiento: {seatLabel}");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine($"Puerta: {gate}");
            AnsiConsole.WriteLine($"Grupo: {group}");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Estado: CHECK-IN COMPLETADO");
            AnsiConsole.WriteLine("====================================");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task CreateCheckInAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]REALIZAR CHECK-IN[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas realizar check-in?", true))
            return;
        var idTicket = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del tiquete (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (idTicket == 0) return;
        try
        {
            if (AppState.IdUserRole != 1)
            {
                var myTicketIds = await GetMyTicketIdsAsync(ct);
                if (!myTicketIds.Contains(idTicket))
                    throw new InvalidOperationException("No puedes hacer check-in para un tiquete que no te pertenece.");
            }

            using var ctx = DbContextFactory.Create();

            // Obtener el vuelo del tiquete para filtrar asientos disponibles
            var tickets = await new GetAllTicketsUseCase(new TicketRepository(ctx)).ExecuteAsync(ct);
            var ticket = tickets.FirstOrDefault(t => t.Id.Value == idTicket);
            if (ticket is null) throw new InvalidOperationException($"No se encontró el tiquete con ID {idTicket}.");

            var bookings = await new GetAllBookingsUseCase(new BookingRepository(ctx)).ExecuteAsync(ct);
            var booking = bookings.FirstOrDefault(b => b.Id.Value == ticket.IdBooking);
            if (booking is null) throw new InvalidOperationException("No se encontró la reserva asociada al tiquete.");

            var seatFlights = await new GetAllSeatFlightsUseCase(new SeatFlightRepository(ctx)).ExecuteAsync(ct);
            var availableSeatIds = seatFlights
                .Where(sf => sf.IdFlight == booking.IdFlight && sf.Available)
                .Select(sf => sf.IdSeat)
                .ToHashSet();

            var allSeats = await new GetAllSeatsUseCase(new SeatRepository(ctx)).ExecuteAsync(ct);
            var flightSeats = allSeats.Where(s => availableSeatIds.Contains(s.Id.Value)).ToList();
            if (!flightSeats.Any()) throw new InvalidOperationException("No hay asientos disponibles en el vuelo de este tiquete.");

            var channels = await new GetAllCheckInChannelsUseCase(new CheckInChannelRepository(ctx)).ExecuteAsync(ct);
            if (!channels.Any()) throw new InvalidOperationException("No hay canales de check-in. Crea uno en Administración.");
            var selChannel = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Selecciona el canal:").PageSize(10)
                    .AddChoices(channels.Select(c => $"{c.Id.Value}. {c.Name.Value}")));
            var idChannel = int.Parse(selChannel.Split(new char[] { '.' })[0]);

            AnsiConsole.MarkupLine($"[grey]Vuelo: {booking.IdFlight} — {flightSeats.Count} asiento(s) disponible(s)[/]");
            var selSeat = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Selecciona el asiento:").PageSize(10)
                    .AddChoices(flightSeats.Select(s => $"{s.Id.Value}. {s.Number.Value}")));
            var idSeat = int.Parse(selSeat.Split(new char[] { '.' })[0]);

            var idStatus = await SelectStatusAsync("CheckIn", ct);
            using var context = DbContextFactory.Create();
            var result = await new CreateCheckInUseCase(new CheckInRepository(context))
                .ExecuteAsync(DateTime.Now, idTicket, idChannel, idSeat, AppState.IdUser, idStatus, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllCheckInsUseCase(new CheckInRepository(context)).ExecuteAsync(ct))
                .Where(c =>
                    c.IdTicket == idTicket &&
                    c.IdChannel == idChannel &&
                    c.IdSeat == idSeat &&
                    c.IdStatus == idStatus &&
                    c.IdUser == AppState.IdUser)
                .OrderByDescending(c => c.Id.Value)
                .Select(c => c.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Check-in realizado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }
}
