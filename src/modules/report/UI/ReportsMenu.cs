using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.report.UI;

public class ReportsMenu : IModuleUI
{
    public string Key => "9";
    public string Title => "Reportes LINQ";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule($"[green]{Key}. {Title.ToUpper()}[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(12)
                    .AddChoices(
                        "1. Vuelos con mayor ocupación",
                        "2. Vuelos con asientos disponibles",
                        "3. Ingresos por aerolínea",
                        "4. Reservas por estado",
                        "5. Tiquetes emitidos por rango de fechas",
                        "6. Clientes con más reservas",
                        "7. Destinos más solicitados",
                        "0. Volver"));

            try
            {
                switch (option)
                {
                    case "1. Vuelos con mayor ocupación":      await FlightOccupancyAsync(cancellationToken);   break;
                    case "2. Vuelos con asientos disponibles": await AvailableFlightsAsync(cancellationToken);  break;
                    case "3. Ingresos por aerolínea":          await RevenueByAirlineAsync(cancellationToken);  break;
                    case "4. Reservas por estado":             await BookingsByStatusAsync(cancellationToken);  break;
                    case "5. Tiquetes emitidos por rango de fechas": await TicketsByDateRangeAsync(cancellationToken); break;
                    case "6. Clientes con más reservas":       await TopCustomersByBookingsAsync(cancellationToken); break;
                    case "7. Destinos más solicitados":        await TopDestinationsByBookingsAsync(cancellationToken); break;
                    case "0. Volver": back = true; break;
                }
            }
            catch (Exception ex)
            {
                EntityPersistenceUiFeedback.Write(ex);
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
                Console.ReadKey();
            }
        }
    }

    private static async Task FlightOccupancyAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]VUELOS CON MAYOR OCUPACIÓN[/]").Centered());
        using var context = DbContextFactory.Create();
        var flights = await new GetAllFlightsUseCase(new FlightRepository(context)).ExecuteAsync(ct);
        var ranked = flights
            .Where(f => f.TotalCapacity.Value > 0)
            .Select(f => new {
                f.Id, Number = f.Number.Value, Date = f.Date.Value,
                Occupied = f.TotalCapacity.Value - f.AvailableSeats.Value,
                Total = f.TotalCapacity.Value,
                Pct = (f.TotalCapacity.Value - f.AvailableSeats.Value) * 100.0 / f.TotalCapacity.Value
            })
            .OrderByDescending(f => f.Pct)
            .Take(20)
            .ToList();

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("Vuelo"); table.AddColumn("Fecha"); table.AddColumn("Ocupados");
        table.AddColumn("Total"); table.AddColumn("Ocupación %");
        foreach (var f in ranked)
            table.AddRow(Markup.Escape(f.Number), f.Date.ToString("yyyy-MM-dd"),
                f.Occupied.ToString(), f.Total.ToString(), $"{f.Pct:F1}%");
        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task AvailableFlightsAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]VUELOS CON ASIENTOS DISPONIBLES[/]").Centered());
        using var context = DbContextFactory.Create();
        var flights = await new GetAllFlightsUseCase(new FlightRepository(context)).ExecuteAsync(ct);
        var routes = await new GetAllRoutesUseCase(new RouteRepository(context)).ExecuteAsync(ct);
        var routeMap = routes.ToDictionary(r => r.Id.Value, r => r.Id.Value.ToString());

        var available = flights
            .Where(f => f.AvailableSeats.Value > 0)
            .OrderBy(f => f.Date.Value).ThenBy(f => f.DepartureTime.Value)
            .ToList();

        if (!available.Any()) { AnsiConsole.MarkupLine("[yellow]No hay vuelos con disponibilidad.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("Vuelo"); table.AddColumn("Fecha"); table.AddColumn("Salida");
            table.AddColumn("Llegada"); table.AddColumn("Disponibles");
            foreach (var f in available)
                table.AddRow(Markup.Escape(f.Number.Value), f.Date.Value.ToString("yyyy-MM-dd"),
                    f.DepartureTime.Value.ToString("HH:mm"), f.ArrivalTime.Value.ToString("HH:mm"),
                    f.AvailableSeats.Value.ToString());
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task RevenueByAirlineAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]INGRESOS POR AEROLÍNEA[/]").Centered());
        using var context = DbContextFactory.Create();
        var payments  = await new GetAllPaymentsUseCase(new PaymentRepository(context)).ExecuteAsync(ct);
        var bookings  = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        var flights   = await new GetAllFlightsUseCase(new FlightRepository(context)).ExecuteAsync(ct);
        var aircrafts = await new GetAllAircraftsUseCase(new AircraftRepository(context)).ExecuteAsync(ct);
        var airlines  = await new GetAllAerolinesUseCase(new AerolineRepository(context)).ExecuteAsync(ct);

        // payment → booking → flight → aircraft → airline
        var bookingFlightMap    = bookings.ToDictionary(b => b.Id.Value, b => b.IdFlight);
        var flightAircraftMap   = flights.ToDictionary(f => f.Id.Value, f => f.IdAircraft);
        var aircraftAirlineMap  = aircrafts.ToDictionary(a => a.Id.Value, a => a.IdAirline);
        var airlineNameMap      = airlines.ToDictionary(a => a.Id.Value, a => a.Name.Value);

        var revenue = payments
            .GroupBy(p =>
            {
                var idFlight   = bookingFlightMap.TryGetValue(p.IdBooking, out var fid)   ? fid  : 0;
                var idAircraft = flightAircraftMap.TryGetValue(idFlight, out var acid)    ? acid : 0;
                var idAirline  = aircraftAirlineMap.TryGetValue(idAircraft, out var alid) ? alid : 0;
                return idAirline;
            })
            .Select(g => new
            {
                AirlineName = airlineNameMap.TryGetValue(g.Key, out var an) ? an : $"Aerolínea {g.Key}",
                Total = g.Sum(p => p.Amount.Value)
            })
            .OrderByDescending(r => r.Total)
            .ToList();

        if (!revenue.Any()) { AnsiConsole.MarkupLine("[yellow]No hay pagos registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("Aerolínea"); table.AddColumn("Total Ingresos");
            foreach (var r in revenue)
                table.AddRow(Markup.Escape(r.AirlineName), r.Total.ToString("C2"));
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine($"\n[bold]Total general: {payments.Sum(p => p.Amount.Value):C2}[/]");
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task BookingsByStatusAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]RESERVAS POR ESTADO[/]").Centered());
        using var context = DbContextFactory.Create();
        var bookings = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var statusMap = statuses.ToDictionary(s => s.Id.Value, s => s.Name.Value);

        var grouped = bookings
            .GroupBy(b => b.IdStatus)
            .Select(g => new {
                Status = statusMap.TryGetValue(g.Key, out var sn) ? sn : g.Key.ToString(),
                Count = g.Count()
            })
            .OrderByDescending(g => g.Count)
            .ToList();

        if (!grouped.Any()) { AnsiConsole.MarkupLine("[yellow]No hay reservas.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("Estado"); table.AddColumn("Cantidad");
            foreach (var g in grouped)
                table.AddRow(Markup.Escape(g.Status), g.Count.ToString());
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine($"\n[bold]Total reservas: {bookings.Count()}[/]");
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task TicketsByDateRangeAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]TIQUETES POR RANGO DE FECHAS[/]").Centered());
        var fromStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Fecha inicio (yyyy-MM-dd):")
                .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                    ? ValidationResult.Success() : ValidationResult.Error("[red]Formato inválido[/]")));
        var toStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Fecha fin (yyyy-MM-dd):")
                .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                    ? ValidationResult.Success() : ValidationResult.Error("[red]Formato inválido[/]")));
        var from = DateOnly.ParseExact(fromStr, "yyyy-MM-dd").ToDateTime(TimeOnly.MinValue);
        var to = DateOnly.ParseExact(toStr, "yyyy-MM-dd").ToDateTime(TimeOnly.MaxValue);

        using var context = DbContextFactory.Create();
        var tickets = await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct);
        var filtered = tickets.Where(t => t.IssueDate.Value >= from && t.IssueDate.Value <= to)
            .OrderBy(t => t.IssueDate.Value).ToList();

        if (!filtered.Any()) { AnsiConsole.MarkupLine("[yellow]No hay tiquetes en ese rango.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Código"); table.AddColumn("Fecha Emisión"); table.AddColumn("Reserva ID");
            foreach (var t in filtered)
                table.AddRow(t.Id.Value.ToString(), Markup.Escape(t.Code.Value),
                    t.IssueDate.Value.ToString("yyyy-MM-dd HH:mm"), t.IdBooking.ToString());
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine($"\n[bold]Total tiquetes: {filtered.Count}[/]");
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task TopCustomersByBookingsAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CLIENTES CON MÁS RESERVAS[/]").Centered());
        using var context = DbContextFactory.Create();
        var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
        var persons = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
        var personNames = persons.ToDictionary(
            p => p.Id.Value,
            p => $"{p.FirstName.Value} {p.LastName.Value}");

        var ranked = links
            .GroupBy(l => l.IdPerson)
            .Select(g => new
            {
                IdPerson = g.Key,
                Reservas = g.Select(x => x.IdBooking).Distinct().Count()
            })
            .OrderByDescending(x => x.Reservas)
            .ThenBy(x => x.IdPerson)
            .Take(25)
            .ToList();

        if (!ranked.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay asociaciones reserva–cliente (BookingCustomer).[/]");
        }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID Persona"); table.AddColumn("Cliente"); table.AddColumn("Reservas distintas");
            foreach (var r in ranked)
            {
                var name = personNames.TryGetValue(r.IdPerson, out var n) ? n : $"Persona {r.IdPerson}";
                table.AddRow(r.IdPerson.ToString(), Markup.Escape(name), r.Reservas.ToString());
            }
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task TopDestinationsByBookingsAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]DESTINOS MÁS SOLICITADOS[/]").Centered());
        using var context = DbContextFactory.Create();
        var bookings = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        var flights = await new GetAllFlightsUseCase(new FlightRepository(context)).ExecuteAsync(ct);
        var routes = await new GetAllRoutesUseCase(new RouteRepository(context)).ExecuteAsync(ct);
        var airports = await new GetAllAirportsUseCase(new AirportRepository(context)).ExecuteAsync(ct);

        var flightRoute = flights.ToDictionary(f => f.Id.Value, f => f.IdRoute);
        var routeDest = routes.ToDictionary(r => r.Id.Value, r => r.DestinationAirport);
        var airportLabel = airports.ToDictionary(
            a => a.Id.Value,
            a => $"{a.Name.Value} ({a.IATACode.Value})");

        var ranked = bookings
            .Select(b =>
            {
                if (!flightRoute.TryGetValue(b.IdFlight, out var idRoute)) return 0;
                return routeDest.TryGetValue(idRoute, out var destId) ? destId : 0;
            })
            .Where(destId => destId > 0)
            .GroupBy(destId => destId)
            .Select(g => new
            {
                IdAirport = g.Key,
                Reservas = g.Count()
            })
            .OrderByDescending(x => x.Reservas)
            .ThenBy(x => x.IdAirport)
            .Take(25)
            .ToList();

        if (!ranked.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay reservas con vuelo/ruta válidos.[/]");
        }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID Aeropuerto"); table.AddColumn("Destino"); table.AddColumn("Reservas");
            foreach (var r in ranked)
            {
                var label = airportLabel.TryGetValue(r.IdAirport, out var lbl) ? lbl : r.IdAirport.ToString();
                table.AddRow(r.IdAirport.ToString(), Markup.Escape(label), r.Reservas.ToString());
            }
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }
}
