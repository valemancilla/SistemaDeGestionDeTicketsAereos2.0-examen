// Registro de abordaje físico: tiquete → Abordado; pase de abordar → Activo (cierre del ciclo del examen).
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;
using Spectre.Console;
using TicketAgg = SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate.Ticket;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.UI;

public static class RegisterPassengerBoardingMenu
{
    private const string TicketType = "Ticket";
    private const string BoardingPassType = "BoardingPass";
    private const string FlightType = "Flight";

    public static async Task RunAsync(CancellationToken ct = default)
    {
        if (AppState.IdUserRole != 1)
        {
            AnsiConsole.MarkupLine(
                "[red]Solo personal autorizado (administrador) puede registrar el abordaje en puerta.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
            return;
        }

        Console.Clear();
        AnsiConsole.Write(new Rule("[green]REGISTRAR ABORDAJE (PUERTA / AERONAVE)[/]").Centered());
        AnsiConsole.MarkupLine(
            "[grey]Pasa el tiquete a [bold]Abordado[/] y el pase de [bold]Generado[/] a [bold]Activo[/] " +
            "cuando el pasajero embarque (Examen 3). Buscá por código de tiquete o de pase.[/]\n");

        var modo = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Criterio:")
                .AddChoices("1. Código de tiquete", "2. Código de pase de abordar"));

        try
        {
            using var context = DbContextFactory.Create();
            var ticketRepo = new TicketRepository(context);
            var bpRepo = new BoardingPassRepository(context);
            var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);

            int? idCheckInListo = ResolveTicketStatusId(statuses, "Check-in realizado");
            int? idAbordado = ResolveTicketStatusId(statuses, "Abordado");
            int? idActivoPase = ResolveBoardingPassStatusId(statuses, "Activo");

            if (idCheckInListo is null || idAbordado is null)
            {
                AnsiConsole.MarkupLine(
                    "[red]Faltan estados de tiquete «Check-in realizado» o «Abordado» en SystemStatus.[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
                return;
            }

            if (idActivoPase is null)
            {
                AnsiConsole.MarkupLine(
                    "[red]Falta el estado «Activo» para BoardingPass en SystemStatus.[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
                return;
            }

            TicketAgg? ticket = null;

            if (modo.StartsWith("1", StringComparison.Ordinal))
            {
                var code = AnsiConsole.Prompt(
                    new TextPrompt<string>("Código de tiquete:")
                        .Validate(s =>
                            string.IsNullOrWhiteSpace(s)
                                ? ValidationResult.Error("[red]Ingresá un código.[/]")
                                : ValidationResult.Success()));
                ticket = await ticketRepo.GetByCodeAsync(code.Trim().ToUpperInvariant(), ct);
            }
            else
            {
                var raw = AnsiConsole.Prompt(
                    new TextPrompt<string>("Código del pase de abordar:")
                        .Validate(s =>
                            string.IsNullOrWhiteSpace(s)
                                ? ValidationResult.Error("[red]Ingresá un código.[/]")
                                : ValidationResult.Success()));
                var pass = await new GetBoardingPassByCodeUseCase(bpRepo).ExecuteAsync(raw, ct);
                if (pass is null)
                {
                    AnsiConsole.MarkupLine("[red]No se encontró un pase con ese código.[/]");
                    ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
                    return;
                }

                ticket = await ticketRepo.GetByIdAsync(TicketId.Create(pass.IdTicket), ct);
            }

            if (ticket is null)
            {
                AnsiConsole.MarkupLine("[red]Tiquete no encontrado.[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
                return;
            }

            if (ticket.IdStatus == idAbordado.Value)
            {
                AnsiConsole.MarkupLine("[yellow]Este tiquete ya figura como Abordado.[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
                return;
            }

            if (ticket.IdStatus != idCheckInListo.Value)
            {
                var name = StatusName(statuses, TicketType, ticket.IdStatus) ?? "—";
                AnsiConsole.MarkupLine(
                    "[red]Solo se puede abordar con tiquete en «Check-in realizado». Estado actual: " +
                    $"[bold]{Markup.Escape(name)}[/].[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
                return;
            }

            var bookRepo = new BookingRepository(context);
            var b = await bookRepo.GetByIdAsync(BookingId.Create(ticket.IdBooking), ct);
            if (b is not null && b.IdFlight > 0)
            {
                var fl = await new FlightRepository(context).GetByIdAsync(FlightId.Create(b.IdFlight), ct);
                if (fl is not null)
                {
                    var flightStatusName = statuses.FirstOrDefault(s =>
                        s.Id.Value == fl.IdStatus &&
                        string.Equals(s.EntityType.Value, FlightType, StringComparison.OrdinalIgnoreCase))?.Name
                        .Value;
                    if (string.Equals(flightStatusName, "Cancelado", StringComparison.OrdinalIgnoreCase))
                    {
                        AnsiConsole.MarkupLine("[red]Vuelo cancelado. No se registra abordaje.[/]");
                        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
                        return;
                    }
                }
            }

            var bp = await new GetBoardingPassByTicketIdUseCase(bpRepo).ExecuteAsync(ticket.Id.Value, ct);
            var passWasAlreadyActivo = bp is not null && bp.IdStatus == idActivoPase.Value;

            await new UpdateTicketUseCase(ticketRepo)
                .ExecuteAsync(
                    ticket.Id.Value,
                    ticket.Code.Value,
                    ticket.IssueDate.Value,
                    ticket.IdBooking,
                    ticket.IdFare,
                    idAbordado.Value,
                    ct);

            if (bp is not null && !passWasAlreadyActivo)
            {
                await new UpdateBoardingPassUseCase(bpRepo).ExecuteAsync(
                    bp.Id.Value,
                    bp.Code.Value,
                    bp.IdTicket,
                    bp.IdSeat,
                    bp.Gate.Value,
                    bp.BoardingTime,
                    bp.CreatedAt,
                    idActivoPase.Value,
                    bp.PassengerFullName,
                    ct);
            }

            await new CreateTicketStatusHistoryUseCase(new TicketStatusHistoryRepository(context))
                .ExecuteAsync(
                    DateTime.Now,
                    "Abordaje registrado (puerta / operación). Tiquete: Abordado; pase: Activo.",
                    ticket.Id.Value,
                    idAbordado.Value,
                    AppState.IdUser,
                    ct);

            await context.SaveChangesAsync(ct);

            AnsiConsole.MarkupLine("[green]Abordaje registrado. Tiquete: [bold]Abordado[/].[/]");
            if (bp is null)
                AnsiConsole.MarkupLine("[grey]No hay pase de abordar en base; solo se actualizó el tiquete.[/]");
            else if (passWasAlreadyActivo)
                AnsiConsole.MarkupLine(
                    "[green]Pase de abordar: ya estaba en estado [bold]Activo[/] en base; se confirmó el abordaje del tiquete.[/]");
            else
                AnsiConsole.MarkupLine("[green]Pase de abordar: [bold]Activo[/] (cierre Generado/Activo del examen).[/]");

            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false, ocultarTeclaPulsada: true);
        }
    }

    private static int? ResolveTicketStatusId(IReadOnlyList<SystemStatus> statuses, string name) =>
        statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, TicketType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, name, StringComparison.OrdinalIgnoreCase))?.Id.Value;

    private static int? ResolveBoardingPassStatusId(IReadOnlyList<SystemStatus> statuses, string name) =>
        statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, BoardingPassType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, name, StringComparison.OrdinalIgnoreCase))?.Id.Value;

    private static string? StatusName(IReadOnlyList<SystemStatus> statuses, string entityType, int id) =>
        statuses.FirstOrDefault(s =>
            s.Id.Value == id && string.Equals(s.EntityType.Value, entityType, StringComparison.OrdinalIgnoreCase))?
            .Name.Value;
}
