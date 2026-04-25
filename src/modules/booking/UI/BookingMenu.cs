using SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Repositories;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.UI;

public sealed class BookingMenu
{
    private const string BookingEntityType = "Booking";
    private const string BookingStatusConfirmed = "Confirmada";
    private const string BookingStatusPending = "Pendiente";
    private const string BookingStatusCanceled = "Cancelada";
    private const string BookingStatusPaid = "Pagada";
    private const string PaymentEntityType = "Payment";
    private const string PaymentStatusApproved = "Aprobado";
    private const string TicketEntityType = "Ticket";
    private const string TicketStatusActive = "Activo";

    private static readonly string[] PseBankChoices =
    [
        "Bancolombia", "Davivienda", "BBVA Colombia", "Banco de Bogotá", "Banco Popular",
        "Scotiabank Colpatria", "Banco Caja Social", "Banco Agrario", "Nequi", "Lulo Bank", "Otro"
    ];

    /// <summary>Combina texto de tarifa/equipaje (ida) con observaciones del usuario; máximo 500 caracteres.</summary>
    private static string? MergeBundledObservation(string? prefix, string? userManual)
    {
        if (string.IsNullOrEmpty(prefix))
            return string.IsNullOrWhiteSpace(userManual) ? null : userManual.Trim();
        var u = string.IsNullOrWhiteSpace(userManual) ? null : userManual.Trim();
        if (u is null)
            return prefix.Length <= 500 ? prefix : prefix[..500];
        var merged = $"{prefix} | {u}";
        return merged.Length <= 500 ? merged : merged[..500];
    }

    /// <param name="skipIntroConfirm">Si es true, no pregunta de nuevo si desea crear la reserva (p. ej. tras confirmar el resumen ida+vuelta).</param>
    /// <param name="seatSelectionFlightLegLabel">Para cliente: «IDA» o «VUELTA» para titular la elección de asientos por tramo; null = solo «este vuelo».</param>
    /// <param name="deferPassengerDetailsCompletion">Cliente ida+vuelta: si es true, no pide datos de pasajeros aquí; el menú de vuelos los solicita antes del resumen final.</param>
    /// <returns>Éxito y ID de la reserva creada (0 si no hubo guardado).</returns>
    public async Task<(bool success, int bookingId)> CreateFromSelectedFlightAsync(
        int idFlight,
        DateTime flightDateTime,
        CancellationToken ct = default,
        int? seatsFromSearch = null,
        string? bundledObservationPrefix = null,
        bool skipIntroConfirm = false,
        string? seatSelectionFlightLegLabel = null,
        bool deferPassengerDetailsCompletion = false)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR RESERVA[/]").Centered());
        if (!string.IsNullOrWhiteSpace(seatSelectionFlightLegLabel) && AppState.IdUserRole != 1)
        {
            var legBanner = seatSelectionFlightLegLabel.Trim().ToUpperInvariant() switch
            {
                "IDA" => "Tramo: vuelo de IDA (ida)",
                "VUELTA" => "Tramo: vuelo de VUELTA (regreso)",
                _ => $"Tramo: {seatSelectionFlightLegLabel.Trim()}",
            };
            AnsiConsole.MarkupLine($"[cyan bold]{Markup.Escape(legBanner)}[/]");
            AnsiConsole.WriteLine();
        }
        if (!skipIntroConfirm && !AnsiConsole.Confirm("¿Deseas crear una reserva para el vuelo seleccionado?", true))
        {
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return (false, 0);
        }

        bool isAdmin = AppState.IdUserRole == 1;
        string code = isAdmin
            ? AnsiConsole.Prompt(
                new TextPrompt<string>("Código de reserva (6-20, solo letras/números, sin guiones). Ej: BK0001:")
                    .Validate(v =>
                        SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject.BookingCode
                                .Create(v) is not null
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Código inválido[/]")))
            : GenerateBookingCode();

        using var context = DbContextFactory.Create();
        var flightForPrompt = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(idFlight, ct);

        if (flightForPrompt.AvailableSeats.Value < 1)
        {
            AnsiConsole.MarkupLine("\n[red]Este vuelo ya no tiene asientos disponibles.[/]");
            AnsiConsole.MarkupLine("[grey]Volvé a buscar vuelos para ver los cupos actualizados.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return (false, 0);
        }

        if (seatsFromSearch is int need && need >= 1 && flightForPrompt.AvailableSeats.Value < need)
        {
            AnsiConsole.MarkupLine(
                $"\n[red]No hay cupo suficiente para las {need} persona(s) de la búsqueda. " +
                $"Cupo a la venta ahora: {flightForPrompt.AvailableSeats.Value} de {flightForPrompt.TotalCapacity.Value} (venta/capacidad del vuelo).[/]");
            AnsiConsole.MarkupLine("[grey]Elegí otro vuelo o volvé a buscar.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return (false, 0);
        }

        const int maxPerBooking = 9;
        var maxSelectable = Math.Min(flightForPrompt.AvailableSeats.Value, maxPerBooking);

        if (seatsFromSearch is >= 1)
            AnsiConsole.MarkupLine("[grey]Es la misma cantidad que en la búsqueda: Enter la confirma o escribí otra (máximo 9 por reserva).[/]");

        var seatPrompt = new TextPrompt<int>(
                $"Número de asientos (cupo a la venta: {flightForPrompt.AvailableSeats.Value}/{flightForPrompt.TotalCapacity.Value}; máximo {maxPerBooking} por reserva):")
            .Validate(v =>
            {
                if (v < 1)
                    return ValidationResult.Error("[red]Debe ser mayor a 0[/]");
                if (v > maxPerBooking)
                    return ValidationResult.Error($"[red]Máximo {maxPerBooking} asientos por reserva.[/]");
                if (v > flightForPrompt.AvailableSeats.Value)
                    return ValidationResult.Error($"[red]Solo hay {flightForPrompt.AvailableSeats.Value} asiento(s) disponible(s) ahora.[/]");
                return ValidationResult.Success();
            });

        if (seatsFromSearch is int s && s >= 1 && s <= maxSelectable)
            seatPrompt.DefaultValue(s);

        var seatCount = AnsiConsole.Prompt(seatPrompt);

        var obs = AnsiConsole.Prompt(
                new TextPrompt<string>("Observaciones (Enter para omitir):")
                    .AllowEmpty())
            .Trim();
        string? observations = string.IsNullOrEmpty(obs) ? null : obs;
        observations = MergeBundledObservation(bundledObservationPrefix, observations);

        var savedOk = false;
        var createdBookingId = 0;
        try
        {
            // Cupo puede haber cambiado mientras completabas el formulario.
            var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(idFlight, ct);
            if (seatCount > flight.AvailableSeats.Value)
                throw new InvalidOperationException($"No hay suficientes asientos disponibles. Disponibles: {flight.AvailableSeats.Value}.");

            var idStatus = isAdmin
                ? await SelectStatusAsync(ct)
                : await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusConfirmed, ct);

            var codeKey = code.Trim().ToUpperInvariant();
            var bookingRepo = new BookingRepository(context);

            if (!isAdmin)
            {
                if (AppState.IdPerson is null)
                {
                    AnsiConsole.MarkupLine("\n[red]No se puede crear la reserva: tu cuenta no tiene un perfil de persona asociado.[/]");
                    ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
                    return (false, 0);
                }

                var seatFlightsPreview = await new GetAllSeatFlightsUseCase(new SeatFlightRepository(context)).ExecuteAsync(ct);
                var freeOnMap = seatFlightsPreview.Count(sf => sf.IdFlight == idFlight && sf.Available);
                if (freeOnMap < seatCount)
                {
                    AnsiConsole.MarkupLine(
                        $"\n[red]No se puede crear la reserva: en el mapa del avión solo hay [bold]{freeOnMap}[/] asiento(s) libre(s), pero la reserva es para [bold]{seatCount}[/] persona(s) (un asiento por persona).[/]");
                    AnsiConsole.MarkupLine("[grey]Un administrador debe generar asientos del vuelo (Vuelos / Aeronaves) o liberar cupos.[/]");
                    ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
                    return (false, 0);
                }

                await using var tx = await context.Database.BeginTransactionAsync(ct);
                try
                {
                    await new CreateBookingUseCase(bookingRepo)
                        .ExecuteAsync(code, flightDateTime, DateOnly.FromDateTime(DateTime.Today), seatCount, observations, idFlight, idStatus, ct);

                    await new UpdateFlightUseCase(new FlightRepository(context))
                        .ExecuteAsync(
                            flight.Id.Value,
                            flight.Number.Value,
                            flight.Date.Value,
                            flight.DepartureTime.Value,
                            flight.ArrivalTime.Value,
                            flight.TotalCapacity.Value,
                            flight.AvailableSeats.Value - seatCount,
                            flight.IdRoute,
                            flight.IdAircraft,
                            flight.IdStatus,
                            flight.IdCrew,
                            flight.IdFare,
                            flight.BoardingGate,
                            ct);

                    await context.SaveChangesAsync(ct);

                    var saved = await bookingRepo.GetByCodeAsync(codeKey, ct)
                        ?? throw new InvalidOperationException("La reserva no pudo recuperarse tras guardar.");

                    await AddPassengersAndSeatsForClientBookingAsync(
                        context, ct, saved.Id.Value, seatCount, seatSelectionFlightLegLabel, deferPassengerDetailsCompletion);

                    var emitOpt = await EmitTicketForNewBookingAsync(context, flight, saved.Id.Value, seatCount, ct);
                    if (emitOpt is null)
                        throw new InvalidOperationException("No se pudo emitir el tiquete; revisá la tarifa del vuelo.");
                    var (tCode, tId) = emitOpt.Value;

                    await tx.CommitAsync(ct);

                    savedOk = true;
                    createdBookingId = saved.Id.Value;
                    if (!deferPassengerDetailsCompletion)
                    {
                        var holderOk = await CompletePassengerDetailsForBookingAsync(saved.Id.Value, ct);
                        if (AppState.IdUserRole != 1 && holderOk)
                            await RunClientCheckoutForBookingsAsync(new[] { saved.Id.Value }, ct);
                    }

                    var legOk = FormatSeatLegSuccessSuffix(seatSelectionFlightLegLabel);
                    if (deferPassengerDetailsCompletion)
                    {
                        var nextHint = string.Equals(seatSelectionFlightLegLabel, "IDA", StringComparison.OrdinalIgnoreCase)
                            ? "[grey]Seguí con el tramo de [bold]vuelta[/]; al terminar ambos tramos te pediremos los datos de cada pasajero y el [bold]resumen del viaje[/].[/]"
                            : "[grey]Al terminar este paso te pediremos los datos de cada pasajero y el [bold]resumen del viaje[/].[/]";
                        AnsiConsole.MarkupLine(
                            $"\n[green]Reserva '[bold]{Markup.Escape(saved.Code.Value)}[/]' creada con ID {saved.Id.Value} " +
                            $"y [bold]{seatCount}[/] asiento(s) elegido(s){legOk}.[/]\n" +
                            $"Tiquete [bold]{Markup.Escape(tCode)}[/] [dim](ID {tId})[/]. {nextHint}\n" +
                            "[grey]El pago lo completás al final del flujo en [bold]Buscar vuelos[/]; en [bold]Gestión de reservas[/] podés solo [bold]ver[/] reservas y pagos.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine(
                            $"\n[green]Reserva '[bold]{Markup.Escape(saved.Code.Value)}[/]' creada con ID {saved.Id.Value} " +
                            $"y [bold]{seatCount}[/] asiento(s) elegido(s){legOk}.[/]\n" +
                            $"Tiquete [bold]{Markup.Escape(tCode)}[/] [dim](ID {tId})[/]. " +
                            "[grey]Si completaste el pago, la reserva queda [bold]Pagada[/]. " +
                            "Podés revisar datos en Gestión de reservas (consulta) si hace falta.[/]");
                    }
                }
                catch
                {
                    await tx.RollbackAsync(ct);
                    throw;
                }
            }
            else
            {
                await new CreateBookingUseCase(bookingRepo)
                    .ExecuteAsync(code, flightDateTime, DateOnly.FromDateTime(DateTime.Today), seatCount, observations, idFlight, idStatus, ct);

                await new UpdateFlightUseCase(new FlightRepository(context))
                    .ExecuteAsync(
                        flight.Id.Value,
                        flight.Number.Value,
                        flight.Date.Value,
                        flight.DepartureTime.Value,
                        flight.ArrivalTime.Value,
                        flight.TotalCapacity.Value,
                        flight.AvailableSeats.Value - seatCount,
                        flight.IdRoute,
                        flight.IdAircraft,
                        flight.IdStatus,
                        flight.IdCrew,
                        flight.IdFare,
                        flight.BoardingGate,
                        ct);

                await context.SaveChangesAsync(ct);
                savedOk = true;

                var saved = await bookingRepo.GetByCodeAsync(codeKey, ct);
                if (saved is not null)
                {
                    createdBookingId = saved.Id.Value;
                    try
                    {
                        var emitA = await EmitTicketForNewBookingAsync(context, flight, saved.Id.Value, seatCount, ct);
                        if (emitA is { } e)
                        {
                            AnsiConsole.MarkupLine(
                                $"\n[green]Reserva '[bold]{Markup.Escape(saved.Code.Value)}[/]' creada con ID {saved.Id.Value}. " +
                                $"Tiquete [bold]{Markup.Escape(e.TicketCode)}[/] (ID {e.TicketId}). " +
                                "[grey]El pago se registra aparte cuando corresponda (menú de pagos).[/][/]");
                        }
                        else
                            AnsiConsole.MarkupLine($"\n[green]Reserva '[bold]{Markup.Escape(saved.Code.Value)}[/]' creada con ID {saved.Id.Value}.[/]");
                    }
                    catch (InvalidOperationException ex)
                    {
                        AnsiConsole.MarkupLine($"\n[green]Reserva '[bold]{Markup.Escape(saved.Code.Value)}[/]' creada con ID {saved.Id.Value}.[/]");
                        AnsiConsole.MarkupLine($"[yellow]Aviso: no se emitió tiquete automático — {Markup.Escape(ex.Message)}[/]");
                    }
                }
                else
                    AnsiConsole.MarkupLine($"\n[green]Reserva registrada correctamente (código [bold]{Markup.Escape(codeKey)}[/]).[/]");

                if (saved is not null && AnsiConsole.Confirm("\n¿Agregar pasajero a esta reserva ahora?", true))
                    await AddPassengerAsync(ct, saved.Id.Value);
            }
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
        return (savedOk, createdBookingId);
    }

    /// <summary>Anula la reserva de ida si no se pudo completar la vuelta en un flujo ida+vuelta (misma lógica que eliminar, sin UI de confirmación).</summary>
    public static async Task<bool> TryUndoOutboundBookingAfterFailedReturnLegAsync(int bookingId, CancellationToken ct)
    {
        if (bookingId <= 0) return true;
        try
        {
            using var context = DbContextFactory.Create();
            var booking = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(bookingId, ct);
            if (booking is null)
            {
                AnsiConsole.MarkupLine("\n[yellow]No se encontró la reserva de ida para revertir (puede haber sido eliminada manualmente).[/]");
                return false;
            }

            await RemoveIssuedTicketsAndPaymentsForBookingAsync(context, bookingId, ct);

            var canceledId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusCanceled, ct);
            var holdsSeats = booking.IdStatus != canceledId;

            if (holdsSeats)
            {
                var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(booking.IdFlight, ct);
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
                        flight.BoardingGate,
                        ct);
            }

            await ReleaseSeatFlightsForBookingAsync(context, booking.Id.Value, booking.IdFlight, ct);

            var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
            foreach (var link in links.Where(l => l.IdBooking == bookingId))
                await new DeleteBookingCustomerUseCase(new BookingCustomerRepository(context)).ExecuteAsync(link.Id.Value, ct);

            var histories = await new GetAllBookingStatusHistoriesUseCase(new BookingStatusHistoryRepository(context)).ExecuteAsync(ct);
            foreach (var hist in histories.Where(x => x.IdBooking == bookingId))
                await new DeleteBookingStatusHistoryUseCase(new BookingStatusHistoryRepository(context)).ExecuteAsync(hist.Id.Value, ct);

            var cancellations = await new GetAllBookingCancellationsUseCase(new BookingCancellationRepository(context)).ExecuteAsync(ct);
            foreach (var cancel in cancellations.Where(c => c.IdBooking == bookingId))
                await new DeleteBookingCancellationUseCase(new BookingCancellationRepository(context)).ExecuteAsync(cancel.Id.Value, ct);

            await new DeleteBookingUseCase(new BookingRepository(context)).ExecuteAsync(bookingId, ct);
            await context.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
            return false;
        }
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        bool isAdmin = AppState.IdUserRole == 1;
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE RESERVAS[/]").Centered());

            if (isAdmin)
            {
                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(9)
                        .AddChoices("1. Crear reserva", "2. Listar reservas", "3. Actualizar reserva",
                                    "4. Agregar pasajero a reserva", "5. Registrar cambio de estado",
                                    "6. Cancelar reserva", "7. Eliminar reserva", "0. Volver"));
                switch (option)
                {
                    case "1. Crear reserva": await CreateAsync(ct); break;
                    case "2. Listar reservas": await ListAsync(ct); break;
                    case "3. Actualizar reserva": await UpdateAsync(ct); break;
                    case "4. Agregar pasajero a reserva": await AddPassengerAsync(ct); break;
                    case "5. Registrar cambio de estado": await AddStatusHistoryAsync(ct); break;
                    case "6. Cancelar reserva": await CancelAsync(ct); break;
                    case "7. Eliminar reserva": await DeleteAsync(ct); break;
                    case "0. Volver": back = true; break;
                }
            }
            else
            {
                AnsiConsole.MarkupLine(
                    "[grey]Consultá tus reservas y pagos; el pago nuevo se hace al finalizar una reserva desde [bold]Buscar vuelos[/]. " +
                    "Los datos de pasajeros se completan en ese mismo flujo al reservar.[/]\n");
                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(5)
                        .AddChoices(
                            "1. Ver mis reservas",
                            "2. Ver pagos realizados",
                            "3. Cancelar mi reserva",
                            "0. Volver"));
                switch (option)
                {
                    case "1. Ver mis reservas":
                        await ClientViewReservationsReadOnlyAsync(ct);
                        break;
                    case "2. Ver pagos realizados":
                        await ClientViewPaymentsReadOnlyAsync(ct);
                        break;
                    case "3. Cancelar mi reserva":
                        await CancelAsync(ct);
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

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var bookings = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        var flights = await new GetAllFlightsUseCase(new FlightRepository(context)).ExecuteAsync(ct);
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var flightMap = flights.ToDictionary(f => f.Id.Value, f => f.Number.Value);
        var statusMap = statuses.ToDictionary(s => s.Id.Value, s => s.Name.Value);

        if (AppState.IdUserRole != 1)
            throw new InvalidOperationException("Listado administrativo: solo personal autorizado.");

        if (!bookings.Any()) { AnsiConsole.MarkupLine("[yellow]No hay reservas registradas.[/]"); }
        else
        {
            // Cargar datos del titular tal como se capturan al crear la reserva (BookingEntity.Holder* / IdHolderPerson).
            var bookingRows = await context.Set<BookingEntity>()
                .AsNoTracking()
                .Select(b => new
                {
                    b.IdBooking,
                    b.HolderEmail,
                    b.HolderPhonePrefix,
                    b.HolderPhone,
                    b.ConsentDataProcessing,
                    b.ConsentMarketing,
                    b.IdHolderPerson
                })
                .ToListAsync(ct);
            var rowById = bookingRows.ToDictionary(x => x.IdBooking);

            // Fallback: si no hay IdHolderPerson (titular/contacto no completado),
            // usar el pasajero marcado como titular de plaza (BookingCustomer.IsPrimary).
            var linksAll = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
            var primaryPersonByBookingId = linksAll
                .Where(l => l.IsPrimary)
                .GroupBy(l => l.IdBooking)
                .ToDictionary(g => g.Key, g => g.First().IdPerson);

            var holderIds = bookingRows
                .Where(x => x.IdHolderPerson.HasValue)
                .Select(x => x.IdHolderPerson!.Value)
                .Distinct()
                .ToList();
            foreach (var pid in primaryPersonByBookingId.Values)
                holderIds.Add(pid);
            holderIds = holderIds.Distinct().ToList();
            var holders = holderIds.Count == 0
                ? new List<PersonEntity>()
                : await context.Set<PersonEntity>()
                    .AsNoTracking()
                    .Where(p => holderIds.Contains(p.IdPerson))
                    .ToListAsync(ct);
            var holderById = holders.ToDictionary(h => h.IdPerson);

            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Código"); table.AddColumn("Vuelo");
            table.AddColumn("Fecha Vuelo"); table.AddColumn("Asientos"); table.AddColumn("Estado");
            table.AddColumn("Titular");
            table.AddColumn("Doc. titular");
            table.AddColumn("Email titular");
            table.AddColumn("Teléfono");
            table.AddColumn("Datos");
            table.AddColumn("Marketing");
            foreach (var b in bookings)
            {
                var flight = flightMap.TryGetValue(b.IdFlight, out var fn) ? fn : b.IdFlight.ToString();
                var status = statusMap.TryGetValue(b.IdStatus, out var sn) ? sn : b.IdStatus.ToString();
                rowById.TryGetValue(b.Id.Value, out var row);
                int? holderPersonId = row?.IdHolderPerson;
                if (holderPersonId is null && primaryPersonByBookingId.TryGetValue(b.Id.Value, out var primPid))
                    holderPersonId = primPid;
                var holder = holderPersonId is int hid && holderById.TryGetValue(hid, out var p) ? p : null;
                var holderName = holder is null ? "—" : $"{holder.FirstName} {holder.LastName}".Trim();
                var holderDoc = holder?.DocumentNumber ?? "—";
                var email = string.IsNullOrWhiteSpace(row?.HolderEmail) ? "—" : row!.HolderEmail!;
                var phone = string.IsNullOrWhiteSpace(row?.HolderPhone) ? "—" : $"{row!.HolderPhonePrefix}{row.HolderPhone}";
                var consentDp = row?.ConsentDataProcessing == true ? "Sí" : "No";
                var consentMk = row?.ConsentMarketing == true ? "Sí" : "No";
                table.AddRow(b.Id.Value.ToString(), Markup.Escape(b.Code.Value),
                    Markup.Escape(flight), b.FlightDate.Value.ToString("yyyy-MM-dd HH:mm"),
                    b.SeatCount.Value.ToString(), Markup.Escape(status),
                    Markup.Escape(holderName),
                    Markup.Escape(holderDoc),
                    Markup.Escape(email),
                    Markup.Escape(phone),
                    Markup.Escape(consentDp),
                    Markup.Escape(consentMk));
            }
            AnsiConsole.Write(table);
        }

        ConsolaPausa.PresionarCualquierTecla();
    }

    /// <summary>Cliente: detalle en paneles de cada reserva propia pagada (solo lectura).</summary>
    private static async Task ClientViewReservationsReadOnlyAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]MIS RESERVAS — CONSULTA[/]").Centered());
        AnsiConsole.MarkupLine(
            "[grey]Solo reservas [bold]ya pagadas[/]. Una caja por reserva (p. ej. ida y vuelta = dos cajas). Para una reserva nueva: [bold]Buscar vuelos[/].[/]\n");

        using var context = DbContextFactory.Create();
        int paidId;
        try
        {
            paidId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusPaid, ct);
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
            AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
            Console.ReadKey();
            return;
        }

        var myIds = await GetMyBookingIdsAsync(ct);
        var bookings = (await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct))
            .Where(b => myIds.Contains(b.Id.Value) && b.IdStatus == paidId)
            .OrderBy(b => b.FlightDate.Value)
            .ThenBy(b => b.Code.Value)
            .ToList();
        var flights = await new GetAllFlightsUseCase(new FlightRepository(context)).ExecuteAsync(ct);
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var flightMap = flights.ToDictionary(f => f.Id.Value, f => f.Number.Value);
        var statusMap = statuses.ToDictionary(s => s.Id.Value, s => s.Name.Value);

        if (bookings.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No tenés reservas pagadas para mostrar.[/]");
            AnsiConsole.MarkupLine(
                "[grey]Si tenés una reserva confirmada sin pagar, completá el pago desde [bold]Buscar vuelos[/] (resumen y pago).[/]");
            AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
            Console.ReadKey();
            return;
        }

        var allLinks = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
        foreach (var bk in bookings)
        {
            var bookingId = bk.Id.Value;
            var row = await context.Set<BookingEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdBooking == bookingId, ct);

            var personIds = allLinks.Where(l => l.IdBooking == bookingId).Select(l => l.IdPerson).Distinct().ToList();
            var persons = personIds.Count == 0
                ? new List<PersonEntity>()
                : await context.Set<PersonEntity>()
                    .AsNoTracking()
                    .Where(p => personIds.Contains(p.IdPerson))
                    .ToListAsync(ct);

            var passengerLines = allLinks
                .Where(l => l.IdBooking == bookingId)
                .OrderByDescending(l => l.IsPrimary)
                .ThenBy(l => l.IdSeat)
                .Select(l =>
                {
                    var p = persons.FirstOrDefault(x => x.IdPerson == l.IdPerson);
                    var name = p is null ? $"Persona ID {l.IdPerson}" : $"{p.FirstName} {p.LastName}".Trim();
                    var prim = l.IsPrimary ? " (titular de plaza)" : "";
                    return $"  • {Markup.Escape(name)}{prim}";
                })
                .ToList();

            var obs = string.IsNullOrWhiteSpace(bk.Observations.Value) ? "—" : Markup.Escape(bk.Observations.Value.Trim());
            var holderEmail = row?.HolderEmail;
            var holderPhone = row is null
                ? null
                : string.IsNullOrWhiteSpace(row.HolderPhone)
                    ? null
                    : $"{row.HolderPhonePrefix} {row.HolderPhone}".Trim();
            var consent = row?.ConsentDataProcessing == true ? "Sí" : "No";

            AnsiConsole.Write(new Panel(
                    $"[bold]Código (PNR):[/] {Markup.Escape(bk.Code.Value)}\n" +
                    $"[bold]Vuelo:[/] {Markup.Escape(flightMap.TryGetValue(bk.IdFlight, out var f2) ? f2 : bk.IdFlight.ToString(CultureInfo.InvariantCulture))}\n" +
                    $"[bold]Fecha del vuelo:[/] {bk.FlightDate.Value:yyyy-MM-dd HH:mm}\n" +
                    $"[bold]Plazas:[/] {bk.SeatCount.Value}\n" +
                    $"[bold]Estado:[/] {Markup.Escape(statusMap.TryGetValue(bk.IdStatus, out var st) ? st : bk.IdStatus.ToString(CultureInfo.InvariantCulture))}\n" +
                    $"[bold]Fecha de creación de la reserva:[/] {bk.CreationDate.Value:yyyy-MM-dd}\n" +
                    $"[bold]Observaciones:[/] {obs}\n" +
                    $"[bold]Correo titular (contacto):[/] {(string.IsNullOrWhiteSpace(holderEmail) ? "—" : Markup.Escape(holderEmail.Trim()))}\n" +
                    $"[bold]Teléfono titular:[/] {(string.IsNullOrWhiteSpace(holderPhone) ? "—" : Markup.Escape(holderPhone))}\n" +
                    $"[bold]Consentimiento datos:[/] {consent}\n" +
                    "[bold]Pasajeros en la reserva:[/]\n" +
                    (passengerLines.Count > 0 ? string.Join('\n', passengerLines) : "  [grey]—[/]") +
                    $"\n[grey]Para cancelar desde el menú, usá solo el [bold]código (PNR)[/]: {Markup.Escape(bk.Code.Value)}[/]")
                .Header($"[cyan]Reserva {Markup.Escape(bk.Code.Value)}[/]")
                .Border(BoxBorder.Rounded));
            AnsiConsole.WriteLine();
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
        Console.ReadKey();
    }

    /// <summary>Cliente: pagos registrados en sus reservas (solo lectura).</summary>
    private static async Task ClientViewPaymentsReadOnlyAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]MIS PAGOS — CONSULTA[/]").Centered());
        AnsiConsole.MarkupLine(
            "[grey]Solo movimientos de pago cuya reserva está en estado [bold]Pagada[/] (compra finalizada) y vinculada a tu perfil.[/]\n");

        using var context = DbContextFactory.Create();
        int paidId;
        try
        {
            paidId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusPaid, ct);
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
            AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
            Console.ReadKey();
            return;
        }

        var myIds = await GetMyBookingIdsAsync(ct);
        var allBookings = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        var myPaidBookingIds = allBookings
            .Where(b => myIds.Contains(b.Id.Value) && b.IdStatus == paidId)
            .Select(b => b.Id.Value)
            .ToHashSet();
        var allPayments = await new GetAllPaymentsUseCase(new PaymentRepository(context)).ExecuteAsync(ct);
        var mine = allPayments
            .Where(p => myPaidBookingIds.Contains(p.IdBooking))
            .OrderByDescending(p => p.Date.Value)
            .ToList();
        var bookings = allBookings;
        var codeByBookingId = bookings.ToDictionary(b => b.Id.Value, b => b.Code.Value);
        var methods = await new GetAllPaymentMethodsUseCase(new PaymentMethodRepository(context)).ExecuteAsync(ct);
        var methodNames = methods.ToDictionary(m => m.Id.Value, m => m.Name.Value);
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var payStatusNames = statuses.ToDictionary(s => s.Id.Value, s => s.Name.Value);

        if (mine.Count == 0)
        {
            AnsiConsole.MarkupLine(
                "[yellow]No hay pagos que listar: solo se muestran reservas [bold]ya pagadas[/]. Completá el pago en [bold]Buscar vuelos[/] para ver el movimiento aquí.[/]");
            AnsiConsole.MarkupLine("[grey]Mientras la reserva no esté [bold]Pagada[/], los intentos de pago no figuran en esta consulta.[/]");
            AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla...[/]");
            Console.ReadKey();
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("ID pago");
        table.AddColumn("Reserva");
        table.AddColumn("PNR");
        table.AddColumn("Monto");
        table.AddColumn("Fecha pago");
        table.AddColumn("Método");
        table.AddColumn("Estado pago");
        foreach (var p in mine)
        {
            var code = codeByBookingId.TryGetValue(p.IdBooking, out var c) ? c : p.IdBooking.ToString(CultureInfo.InvariantCulture);
            var mn = methodNames.TryGetValue(p.IdPaymentMethod, out var m) ? m : p.IdPaymentMethod.ToString(CultureInfo.InvariantCulture);
            var sn = payStatusNames.TryGetValue(p.IdStatus, out var s) ? s : p.IdStatus.ToString(CultureInfo.InvariantCulture);
            table.AddRow(
                p.Id.Value.ToString(),
                p.IdBooking.ToString(CultureInfo.InvariantCulture),
                Markup.Escape(code),
                p.Amount.Value.ToString("N2", CultureInfo.InvariantCulture),
                p.Date.Value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                Markup.Escape(mn),
                Markup.Escape(sn));
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla...[/]");
        Console.ReadKey();
    }

    /// <summary>Cliente: menú de selección solo para reservas confirmadas con BookingCustomer = plazas (sin tabla ni ID a mano).</summary>
    private static async Task ClientSelectConfirmedBookingForPassengerDataAsync(
        AppDbContext context,
        IReadOnlyList<Booking> myBookings,
        IReadOnlyDictionary<int, string> flightMap,
        IReadOnlyDictionary<int, string> statusMap,
        CancellationToken ct)
    {
        int confirmedId;
        try
        {
            confirmedId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusConfirmed, ct);
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
            return;
        }

        var allLinks = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);

        static int CustomerRowCount(IReadOnlyList<BookingCustomer> links, int idBooking) =>
            links.Count(l => l.IdBooking == idBooking);

        var eligible = myBookings
            .Where(b => b.IdStatus == confirmedId)
            .Where(b =>
            {
                var n = CustomerRowCount(allLinks, b.Id.Value);
                return n > 0 && n == b.SeatCount.Value;
            })
            .OrderByDescending(b => b.FlightDate.Value)
            .ToList();

        if (eligible.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay reservas listas para este paso.[/]");
            AnsiConsole.MarkupLine(
                "[grey]Hace falta que la reserva esté en estado «Confirmada» y que exista un registro de pasajero/asiento " +
                "por cada plaza reservada (lo habitual tras confirmar y elegir todos los asientos).[/]");
            return;
        }

        AnsiConsole.MarkupLine("[cyan]Seleccioná la reserva para completar los datos de los pasajeros:[/]");
        var choices = new List<string> { "0 · Volver" };
        foreach (var b in eligible)
        {
            var flightNo = flightMap.TryGetValue(b.IdFlight, out var fn) ? fn : b.IdFlight.ToString();
            var statusName = statusMap.TryGetValue(b.IdStatus, out var sn) ? sn : b.IdStatus.ToString();
            choices.Add(
                $"{b.Id.Value} · {b.Code.Value} · Vuelo {flightNo} · {b.FlightDate.Value:yyyy-MM-dd HH:mm} · " +
                $"{b.SeatCount.Value} plaza(s) · {statusName}");
        }

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Reserva confirmada con asientos:")
                .PageSize(12)
                .AddChoices(choices));

        if (selected.StartsWith("0 ·", StringComparison.Ordinal))
            return;

        var head = selected.Split(new[] { " · " }, 2, StringSplitOptions.None)[0].Trim();
        if (!int.TryParse(head, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var bookingId))
            return;

        if (eligible.All(b => b.Id.Value != bookingId))
            return;

        await CompletePassengerDetailsForBookingAsync(bookingId, ct);
    }

    private static List<BookingCustomer> OrderBookingCustomersForBooking(int idBooking, IReadOnlyList<BookingCustomer> allLinks) =>
        allLinks
            .Where(l => l.IdBooking == idBooking)
            .OrderByDescending(l => l.IsPrimary)
            .ThenBy(l => l.IdSeat)
            .ThenBy(l => l.Id.Value)
            .ToList();

    /// <summary>Ida y vuelta: un solo formulario por plaza; los mismos datos aplican a ambas reservas. Luego titular y contacto.</summary>
    /// <returns><see langword="true"/> si el cliente completó titular, contacto y consentimiento (listo para pagar); en caso contrario <see langword="false"/>.</returns>
    public static async Task<bool> CompletePassengerDetailsForRoundTripBookingsAsync(int idaBookingId, int vueBookingId, CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]COMPLETAR DATOS DE PASAJEROS (IDA + VUELTA)[/]").Centered());
        AnsiConsole.MarkupLine(
            "[cyan]Un solo formulario:[/] [grey]la cantidad de pasajeros ya quedó definida al elegir asientos " +
            "([bold]no[/] se vuelve a preguntar). Los datos de cada viajero se aplican al vuelo de ida y al de vuelta.[/]");
        AnsiConsole.MarkupLine(
            "[grey]Ingresá nombre y apellidos tal como figuran en el pasaporte o documento de identidad.[/]\n");

        var readyForPayment = false;
        try
        {
            using var context = DbContextFactory.Create();
            var bookingIda = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(idaBookingId, ct);
            var bookingVue = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(vueBookingId, ct);
            if (bookingIda is null || bookingVue is null)
            {
                AnsiConsole.MarkupLine("[yellow]No se encontró una de las reservas de ida/vuelta.[/]");
                return false;
            }

            var canceledId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusCanceled, ct);
            if (bookingIda.IdStatus == canceledId || bookingVue.IdStatus == canceledId)
            {
                AnsiConsole.MarkupLine("[yellow]Una de las reservas está cancelada; no se pueden editar pasajeros.[/]");
                return false;
            }

            var confirmedId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusConfirmed, ct);
            if (bookingIda.IdStatus != confirmedId || bookingVue.IdStatus != confirmedId)
            {
                AnsiConsole.MarkupLine("[yellow]Ambas reservas deben estar confirmadas para completar pasajeros.[/]");
                return false;
            }

            if (bookingIda.SeatCount.Value != bookingVue.SeatCount.Value)
            {
                AnsiConsole.MarkupLine(
                    "[yellow]La ida y la vuelta no tienen la misma cantidad de plazas; no se puede usar el formulario unificado. Revisá cada reserva con un administrador o cancelá y volvé a reservar.[/]");
                return false;
            }

            var allLinks = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
            var orderedIda = OrderBookingCustomersForBooking(idaBookingId, allLinks);
            var orderedVue = OrderBookingCustomersForBooking(vueBookingId, allLinks);
            var n = bookingIda.SeatCount.Value;
            if (orderedIda.Count != n || orderedVue.Count != n)
            {
                AnsiConsole.MarkupLine(
                    "[yellow]Faltan registros de asiento/pasajero en ida o vuelta; revisá con un administrador.[/]");
                return false;
            }

            var passengerExtraLines = new List<string?>(n);
            var displayNames = new List<string>(n);
            var personIdPerSlot = new List<int>(n);

            for (var i = 0; i < n; i++)
            {
                allLinks = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
                orderedIda = OrderBookingCustomersForBooking(idaBookingId, allLinks);
                orderedVue = OrderBookingCustomersForBooking(vueBookingId, allLinks);
                var linkI = orderedIda[i];
                var linkV = orderedVue[i];
                var label = $"Pasajero {i + 1} de {n} (aplica a ida y vuelta)";
                Console.Clear();
                AnsiConsole.Write(new Rule("[bold cyan]Información personal de los pasajeros[/]").Centered());
                AnsiConsole.MarkupLine(
                    "[grey]Datos de identidad y contacto de cada viajero para la reserva y el tiquete; " +
                    "los mismos datos valen para el vuelo de ida y el de vuelta.[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Rule($"[green]{Markup.Escape(label)}[/]").Centered());
                AnsiConsole.MarkupLine("[grey]Ingresá el nombre y apellido tal como aparecen en el pasaporte o documento de identidad.[/]");
                AnsiConsole.WriteLine();

                var (firstName, lastName, idGender, birthDate, idCountry, idDocType, documentNumber, passengerExtraLinesItem) =
                    await PromptPassengerIdentityFieldsAsync(label, ct);
                passengerExtraLines.Add(passengerExtraLinesItem);
                displayNames.Add($"{firstName.Trim()} {lastName.Trim()}".Trim());

                var slotPersonId = await PersistPassengerSlotPersonAsync(
                    context,
                    linkI,
                    linkV,
                    slotIndex: i,
                    firstName.Trim(),
                    lastName.Trim(),
                    birthDate,
                    documentNumber.Trim(),
                    idDocType,
                    idGender,
                    idCountry,
                    sharedPlaceholderPersonId: 0,
                    ct);
                personIdPerSlot.Add(slotPersonId);

                await context.SaveChangesAsync(ct);
                AnsiConsole.MarkupLine($"[green]{Markup.Escape(label)}: datos guardados para ambos tramos.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar con el siguiente…[/]");
                Console.ReadKey();
            }

            var mergedIda = AppendPassengerPreferenceLines(bookingIda.Observations.Value, passengerExtraLines);
            var mergedVue = AppendPassengerPreferenceLines(bookingVue.Observations.Value, passengerExtraLines);
            if (!string.Equals(mergedIda, bookingIda.Observations.Value, StringComparison.Ordinal))
            {
                await new UpdateBookingUseCase(new BookingRepository(context))
                    .ExecuteAsync(
                        bookingIda.Id.Value,
                        bookingIda.Code.Value,
                        bookingIda.FlightDate.Value,
                        bookingIda.CreationDate.Value,
                        bookingIda.SeatCount.Value,
                        mergedIda,
                        bookingIda.IdFlight,
                        bookingIda.IdStatus,
                        ct);
            }

            if (!string.Equals(mergedVue, bookingVue.Observations.Value, StringComparison.Ordinal))
            {
                await new UpdateBookingUseCase(new BookingRepository(context))
                    .ExecuteAsync(
                        bookingVue.Id.Value,
                        bookingVue.Code.Value,
                        bookingVue.FlightDate.Value,
                        bookingVue.CreationDate.Value,
                        bookingVue.SeatCount.Value,
                        mergedVue,
                        bookingVue.IdFlight,
                        bookingVue.IdStatus,
                        ct);
            }

            await context.SaveChangesAsync(ct);

            allLinks = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
            orderedIda = OrderBookingCustomersForBooking(idaBookingId, allLinks);
            orderedVue = OrderBookingCustomersForBooking(vueBookingId, allLinks);

            if (AppState.IdUserRole != 1)
            {
                readyForPayment = await PromptTitularSelectionAndHolderContactAsync(
                    context,
                    new[] { idaBookingId, vueBookingId },
                    displayNames,
                    personIdPerSlot,
                    new IReadOnlyList<BookingCustomer>[] { orderedIda, orderedVue },
                    ct);
            }
            else
                readyForPayment = true;

            if (readyForPayment)
                AnsiConsole.MarkupLine("\n[green]Datos de pasajeros y titular registrados para ida y vuelta.[/]");
            else
                AnsiConsole.MarkupLine(
                    "\n[yellow]Pasajeros guardados, pero sin aceptación de datos del titular no podés pagar. " +
                    "Completá titular, contacto y consentimiento en el flujo de [bold]Buscar vuelos[/] antes del pago, o cancelá y volvé a reservar.[/]");

            return readyForPayment;
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
            return false;
        }
        finally
        {
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
        }
    }

    /// <summary>Flujo cliente: un bloque por plaza (BookingCustomer), como en el formulario de pasajeros del sitio. También se usa desde la reserva en curso o desde «Mis reservas».</summary>
    /// <returns><see langword="true"/> si el cliente completó titular, contacto y consentimiento (listo para pagar); en caso contrario <see langword="false"/>.</returns>
    public static async Task<bool> CompletePassengerDetailsForBookingAsync(int bookingId, CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]COMPLETAR DATOS DE PASAJEROS[/]").Centered());
        AnsiConsole.MarkupLine(
            "[grey]La cantidad de pasajeros ya está definida por los asientos elegidos; solo completá los datos de cada uno.[/]\n");

        var readyForPayment = false;
        try
        {
            using var context = DbContextFactory.Create();
            var booking = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(bookingId, ct);
            if (booking is null)
            {
                AnsiConsole.MarkupLine($"[yellow]No existe la reserva con ID {bookingId}.[/]");
                return false;
            }

            var canceledId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusCanceled, ct);
            if (booking.IdStatus == canceledId)
            {
                AnsiConsole.MarkupLine("[yellow]Esta reserva está cancelada; no se pueden editar pasajeros.[/]");
                return false;
            }

            var confirmedId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusConfirmed, ct);
            if (booking.IdStatus != confirmedId)
            {
                AnsiConsole.MarkupLine("[yellow]Solo se pueden completar pasajeros en reservas confirmadas (con asientos ya asignados en el flujo anterior).[/]");
                return false;
            }

            var allLinks = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
            var ordered = OrderBookingCustomersForBooking(bookingId, allLinks);

            if (ordered.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Esta reserva no tiene asientos/pasajeros asociados. Contactá al administrador.[/]");
                return false;
            }

            if (ordered.Count != booking.SeatCount.Value)
            {
                AnsiConsole.MarkupLine(
                    $"[yellow]La reserva indica {booking.SeatCount.Value} plaza(s) pero hay {ordered.Count} registro(s) de pasajero/asiento. " +
                    "Este paso requiere que coincidan; revisá la reserva con un administrador.[/]");
                return false;
            }

            var primary = ordered.FirstOrDefault(l => l.IsPrimary) ?? ordered[0];
            var sharedPlaceholderPersonId = primary.IdPerson;
            var passengerExtraLines = new List<string?>(ordered.Count);
            var displayNames = new List<string>(ordered.Count);
            var personIdPerSlot = new List<int>(ordered.Count);

            for (var i = 0; i < ordered.Count; i++)
            {
                allLinks = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
                ordered = OrderBookingCustomersForBooking(bookingId, allLinks);
                var link = ordered[i];
                var label = $"Pasajero {i + 1} de {ordered.Count}";
                Console.Clear();
                AnsiConsole.Write(new Rule("[bold cyan]Información personal de los pasajeros[/]").Centered());
                AnsiConsole.MarkupLine(
                    "[grey]Datos de identidad de cada viajero para esta reserva y su tiquete en el vuelo que elegiste.[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Rule($"[green]{Markup.Escape(label)}[/]").Centered());
                AnsiConsole.MarkupLine(
                    "[grey]Ingresá el nombre y primer apellido (de cada pasajero) tal y como aparecen en el pasaporte o documento de identidad.[/]");
                AnsiConsole.WriteLine();

                var (firstName, lastName, idGender, birthDate, idCountry, idDocType, documentNumber, passengerExtraLinesItem) =
                    await PromptPassengerIdentityFieldsAsync(label, ct);
                passengerExtraLines.Add(passengerExtraLinesItem);
                displayNames.Add($"{firstName.Trim()} {lastName.Trim()}".Trim());

                var slotPersonId = await PersistPassengerSlotPersonAsync(
                    context,
                    link,
                    linkReturn: null,
                    slotIndex: i,
                    firstName.Trim(),
                    lastName.Trim(),
                    birthDate,
                    documentNumber.Trim(),
                    idDocType,
                    idGender,
                    idCountry,
                    sharedPlaceholderPersonId,
                    ct);
                personIdPerSlot.Add(slotPersonId);

                await context.SaveChangesAsync(ct);
                AnsiConsole.MarkupLine($"[green]{Markup.Escape(label)}: datos guardados.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar con el siguiente…[/]");
                Console.ReadKey();
            }

            var mergedObs = AppendPassengerPreferenceLines(booking.Observations.Value, passengerExtraLines);
            if (!string.Equals(mergedObs, booking.Observations.Value, StringComparison.Ordinal))
            {
                await new UpdateBookingUseCase(new BookingRepository(context))
                    .ExecuteAsync(
                        booking.Id.Value,
                        booking.Code.Value,
                        booking.FlightDate.Value,
                        booking.CreationDate.Value,
                        booking.SeatCount.Value,
                        mergedObs,
                        booking.IdFlight,
                        booking.IdStatus,
                        ct);
                await context.SaveChangesAsync(ct);
            }

            allLinks = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
            ordered = OrderBookingCustomersForBooking(bookingId, allLinks);

            if (AppState.IdUserRole != 1)
            {
                readyForPayment = await PromptTitularSelectionAndHolderContactAsync(
                    context,
                    new[] { bookingId },
                    displayNames,
                    personIdPerSlot,
                    new IReadOnlyList<BookingCustomer>[] { ordered },
                    ct);
            }
            else
                readyForPayment = true;

            if (readyForPayment)
                AnsiConsole.MarkupLine("\n[green]Datos de pasajeros registrados para la reserva.[/]");
            else
                AnsiConsole.MarkupLine(
                    "\n[yellow]Pasajeros guardados, pero sin aceptación de datos del titular no podés pagar. " +
                    "Completá titular, contacto y consentimiento en el flujo de [bold]Buscar vuelos[/] antes del pago, o cancelá y volvé a reservar.[/]");

            return readyForPayment;
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
            return false;
        }
        finally
        {
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
        }
    }

    private static async Task<(string FirstName, string LastName, int IdGender, DateOnly BirthDate, int IdCountry, int IdDocType, string DocumentNumber, string? ExtraObsLine)>
        PromptPassengerIdentityFieldsAsync(string slotLabel, CancellationToken ct)
    {
        var firstName = AnsiConsole.Prompt(
            new TextPrompt<string>("Nombre(s) (obligatorio):")
                .Validate(s => !string.IsNullOrWhiteSpace(s)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Obligatorio[/]")));
        var lastName = AnsiConsole.Prompt(
            new TextPrompt<string>("Apellido(s) (obligatorio):")
                .Validate(s => !string.IsNullOrWhiteSpace(s)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Obligatorio[/]")));

        var idGender = await SelectGenderForPassengerFormAsync(ct);
        var birthDate = PromptPassengerBirthDateOnly();
        AnsiConsole.MarkupLine("[grey]Nacionalidad del documento de viaje[/]");
        var idCountry = await SelectCountryNationalityForPassengerFormAsync(ct);
        var idDocType = await SelectDocumentTypeForPassengerFormAsync(ct);
        var documentNumber = AnsiConsole.Prompt(
            new TextPrompt<string>("Número de documento (obligatorio):")
                .Validate(s => !string.IsNullOrWhiteSpace(s)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Obligatorio[/]")));

        var ff = AnsiConsole.Prompt(
            new TextPrompt<string>("Número de viajero frecuente (opcional, Enter para omitir):")
                .AllowEmpty());
        var assistance = AnsiConsole.Confirm("¿Necesitás asistencia especial? (opcional)", false);

        var extras = new List<string>();
        if (!string.IsNullOrWhiteSpace(ff))
            extras.Add($"FF:{ff.Trim()}");
        if (assistance)
            extras.Add("Asistencia especial:Sí");
        var extraLine = extras.Count > 0 ? $"{slotLabel}: {string.Join(", ", extras)}" : null;
        return (firstName, lastName, idGender, birthDate, idCountry, idDocType, documentNumber, extraLine);
    }

    /// <summary>Actualiza o crea la persona del pasajero y enlaza el/los BookingCustomer del mismo slot (ida+vuelta comparten persona).</summary>
    private static async Task<int> PersistPassengerSlotPersonAsync(
        AppDbContext context,
        BookingCustomer linkOut,
        BookingCustomer? linkReturn,
        int slotIndex,
        string firstName,
        string lastName,
        DateOnly birthDate,
        string documentNumber,
        int idDocType,
        int idGender,
        int idCountry,
        int sharedPlaceholderPersonId,
        CancellationToken ct)
    {
        var personRepo = new PersonRepository(context);
        var bcRepo = new BookingCustomerRepository(context);
        var personOut = linkOut.IdPerson;
        var personBack = linkReturn?.IdPerson;

        if (linkReturn is not null)
        {
            if (personOut == personBack)
            {
                if (slotIndex == 0)
                {
                    await new UpdatePersonUseCase(personRepo)
                        .ExecuteAsync(personOut, firstName, lastName, birthDate, documentNumber, idDocType, idGender, idCountry, null, ct);
                    return personOut;
                }

                await new CreatePersonUseCase(personRepo)
                    .ExecuteAsync(firstName, lastName, birthDate, documentNumber, idDocType, idGender, idCountry, null, ct);
                await context.SaveChangesAsync(ct);
                var created = await personRepo.GetByDocumentAsync(idDocType, documentNumber, ct)
                    ?? throw new InvalidOperationException("No se pudo recuperar la persona recién creada.");
                await new UpdateBookingCustomerUseCase(bcRepo)
                    .ExecuteAsync(linkOut.Id.Value, linkOut.AssociationDate.Value, linkOut.IdBooking, linkOut.IdUser,
                        created.Id.Value, linkOut.IdSeat, linkOut.IsPrimary, ct);
                await new UpdateBookingCustomerUseCase(bcRepo)
                    .ExecuteAsync(linkReturn.Id.Value, linkReturn.AssociationDate.Value, linkReturn.IdBooking, linkReturn.IdUser,
                        created.Id.Value, linkReturn.IdSeat, linkReturn.IsPrimary, ct);
                return created.Id.Value;
            }

            await new CreatePersonUseCase(personRepo)
                .ExecuteAsync(firstName, lastName, birthDate, documentNumber, idDocType, idGender, idCountry, null, ct);
            await context.SaveChangesAsync(ct);
            var createdSplit = await personRepo.GetByDocumentAsync(idDocType, documentNumber, ct)
                ?? throw new InvalidOperationException("No se pudo recuperar la persona recién creada.");
            await new UpdateBookingCustomerUseCase(bcRepo)
                .ExecuteAsync(linkOut.Id.Value, linkOut.AssociationDate.Value, linkOut.IdBooking, linkOut.IdUser,
                    createdSplit.Id.Value, linkOut.IdSeat, linkOut.IsPrimary, ct);
            await new UpdateBookingCustomerUseCase(bcRepo)
                .ExecuteAsync(linkReturn.Id.Value, linkReturn.AssociationDate.Value, linkReturn.IdBooking, linkReturn.IdUser,
                    createdSplit.Id.Value, linkReturn.IdSeat, linkReturn.IsPrimary, ct);
            return createdSplit.Id.Value;
        }

        if (linkOut.IdPerson == sharedPlaceholderPersonId && slotIndex == 0)
        {
            await new UpdatePersonUseCase(personRepo)
                .ExecuteAsync(linkOut.IdPerson, firstName, lastName, birthDate, documentNumber, idDocType, idGender, idCountry, null, ct);
            return linkOut.IdPerson;
        }

        if (linkOut.IdPerson == sharedPlaceholderPersonId)
        {
            await new CreatePersonUseCase(personRepo)
                .ExecuteAsync(firstName, lastName, birthDate, documentNumber, idDocType, idGender, idCountry, null, ct);
            await context.SaveChangesAsync(ct);
            var createdSingle = await personRepo.GetByDocumentAsync(idDocType, documentNumber, ct)
                ?? throw new InvalidOperationException("No se pudo recuperar la persona recién creada.");
            await new UpdateBookingCustomerUseCase(bcRepo)
                .ExecuteAsync(linkOut.Id.Value, linkOut.AssociationDate.Value, linkOut.IdBooking, linkOut.IdUser,
                    createdSingle.Id.Value, linkOut.IdSeat, linkOut.IsPrimary, ct);
            return createdSingle.Id.Value;
        }

        await new UpdatePersonUseCase(personRepo)
            .ExecuteAsync(linkOut.IdPerson, firstName, lastName, birthDate, documentNumber, idDocType, idGender, idCountry, null, ct);
        return linkOut.IdPerson;
    }

    /// <summary>
    /// Igual que en el sitio: primero titular de la reserva (elección de pasajero si hay varios), luego contacto del titular.
    /// </summary>
    /// <returns><see langword="true"/> si el titular quedó con contacto y consentimiento guardados; si no acepta datos, <see langword="false"/> (no se puede pagar).</returns>
    private static async Task<bool> PromptTitularSelectionAndHolderContactAsync(
        AppDbContext context,
        int[] bookingIds,
        IReadOnlyList<string> displayNames,
        IReadOnlyList<int> personIdPerSlot,
        IReadOnlyList<IReadOnlyList<BookingCustomer>> orderedsToFlagTitular,
        CancellationToken ct)
    {
        if (bookingIds.Length == 0)
            return true;
        if (displayNames.Count == 0 || personIdPerSlot.Count != displayNames.Count)
            throw new InvalidOperationException("Los datos de pasajeros no coinciden para asignar titular.");

        Console.Clear();
        AnsiConsole.Write(new Rule("[green]Titular de la reserva[/]").Centered());
        AnsiConsole.MarkupLine(
            "[grey]Será la persona a la cual contactaremos para informarle sobre la reserva y administrar reembolsos. " +
            "El sistema [bold]no[/] deduce al titular por tu usuario de sesión.[/]\n");

        int titularIdx;
        if (displayNames.Count == 1)
        {
            titularIdx = 0;
            AnsiConsole.MarkupLine(
                $"[grey]Pasajero titular:[/] [bold]{Markup.Escape(displayNames[0])}[/] [dim](única plaza en esta reserva)[/]\n");
        }
        else
        {
            var choices = displayNames.Select((n, idx) => $"{idx + 1}. {Markup.Escape(n)}").ToList();
            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Pasajero (titular de la reserva):")
                    .PageSize(10)
                    .AddChoices(choices));
            var head = pick.Split('.', 2)[0].Trim();
            if (!int.TryParse(head, NumberStyles.Integer, CultureInfo.InvariantCulture, out var oneBased) ||
                oneBased < 1 || oneBased > displayNames.Count)
                titularIdx = 0;
            else
                titularIdx = oneBased - 1;
            AnsiConsole.WriteLine();
        }

        foreach (var ordered in orderedsToFlagTitular)
            await ApplyTitularFlagsToOrderedLinksAsync(context, ordered, titularIdx, ct);
        await context.SaveChangesAsync(ct);

        var holderPersonId = personIdPerSlot[titularIdx];

        AnsiConsole.Write(new Rule("[green]Contacto del titular[/]").Centered());
        AnsiConsole.MarkupLine(
            "[grey]Correo y teléfono para gestionar la reserva (se guardan aquí). " +
            "[bold]No[/] se rellenan solos desde tu usuario de sesión: los ingresás ahora.[/]\n");

        var email = AnsiConsole.Prompt(
            new TextPrompt<string>("Correo electrónico del titular (obligatorio):")
                .Validate(s =>
                    !string.IsNullOrWhiteSpace(s) && s.Contains('@', StringComparison.Ordinal)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Correo inválido[/]")));
        var emailConfirm = AnsiConsole.Prompt(
            new TextPrompt<string>("Confirma tu correo electrónico:")
                .Validate(s =>
                    string.Equals(s.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]No coincide con el correo ingresado[/]")));

        var prefix = AnsiConsole.Prompt(
            new TextPrompt<string>("Prefijo telefónico (ej. +57):")
                .DefaultValue("+57"));
        var phone = AnsiConsole.Prompt(
            new TextPrompt<string>("Teléfono móvil (obligatorio):")
                .Validate(s => !string.IsNullOrWhiteSpace(s) ? ValidationResult.Success() : ValidationResult.Error("[red]Obligatorio[/]")));

        AnsiConsole.MarkupLine("\n[grey]Tratamiento de datos personales (obligatorio para continuar)[/]");
        AnsiConsole.MarkupLine("[grey]La opción predeterminada es [bold]Sí[/]: al pulsar Enter se toma como aceptación.[/]");
        if (!AnsiConsole.Confirm("Acepto el tratamiento de mis datos personales conforme a la política de la aerolínea.", true))
        {
            AnsiConsole.MarkupLine(
                "[yellow]Sin esta aceptación no se guardan el correo ni el teléfono en la reserva. " +
                "No podés continuar al pago: tenés que aceptar el tratamiento de datos en el flujo de reserva ([bold]Buscar vuelos[/], titular y contacto) " +
                "o cancelar la reserva y volver a buscar vuelos.[/]");
            return false;
        }

        foreach (var id in bookingIds)
        {
            var entity = await context.Set<BookingEntity>().FirstOrDefaultAsync(b => b.IdBooking == id, ct);
            if (entity is null)
                continue;
            entity.IdHolderPerson = holderPersonId;
            entity.HolderEmail = email.Trim();
            entity.HolderPhonePrefix = string.IsNullOrWhiteSpace(prefix) ? "+57" : prefix.Trim();
            entity.HolderPhone = phone.Trim();
            entity.ConsentDataProcessing = true;
            entity.ConsentMarketing = false;
        }

        await context.SaveChangesAsync(ct);
        AnsiConsole.MarkupLine("[green]Contacto y aceptación de datos guardados.[/]");
        return true;
    }

    private static async Task ApplyTitularFlagsToOrderedLinksAsync(
        AppDbContext context,
        IReadOnlyList<BookingCustomer> ordered,
        int titularSlotIndex,
        CancellationToken ct)
    {
        var bcRepo = new BookingCustomerRepository(context);
        for (var j = 0; j < ordered.Count; j++)
        {
            var l = ordered[j];
            var isTitular = j == titularSlotIndex;
            await new UpdateBookingCustomerUseCase(bcRepo)
                .ExecuteAsync(l.Id.Value, l.AssociationDate.Value, l.IdBooking, l.IdUser, l.IdPerson, l.IdSeat, isTitular, ct);
        }
    }

    /// <summary>Mismo bloque de importes que en «Confirmar y pagar»: total y reparto por reserva (COP).</summary>
    private static async Task<decimal[]> ComputeAndDisplayClientPaymentSummaryAsync(
        AppDbContext context,
        int[] distinctBookingIds,
        CancellationToken ct)
    {
        var distinct = distinctBookingIds.Distinct().ToArray();
        var rawPerBooking = new List<decimal>(distinct.Length);
        foreach (var id in distinct)
            rawPerBooking.Add(await ComputeBookingPayableRawTotalAsync(context, id, ct));

        var parts = BuildCopPaymentPartsFromRawTotals(rawPerBooking);
        var grandTotal = parts.Sum();

        AnsiConsole.MarkupLine(
            $"[bold]Total a pagar:[/] [bold]{Markup.Escape(FareSeatClassPricingHelper.FormatPriceCopColombia(grandTotal))}[/]");
        if (distinct.Length > 1)
        {
            for (var i = 0; i < distinct.Length; i++)
            {
                AnsiConsole.MarkupLine(
                    $"[grey]Reserva {distinct[i]}:[/] {Markup.Escape(FareSeatClassPricingHelper.FormatPriceCopColombia(parts[i]))}");
            }
        }

        AnsiConsole.MarkupLine(string.Empty);
        return parts;
    }

    /// <summary>Cliente: elegir reserva confirmada con titular y datos listos; mismo criterio que el checkout.</summary>
    private static async Task<int?> PromptClientBookingIdEligibleForCheckoutAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        int confirmedId;
        try
        {
            confirmedId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusConfirmed, ct);
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
            return null;
        }

        var myIds = await GetMyBookingIdsAsync(ct);
        var allBookings = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        var flights = await new GetAllFlightsUseCase(new FlightRepository(context)).ExecuteAsync(ct);
        var flightMap = flights.ToDictionary(f => f.Id.Value, f => f.Number.Value);
        var mine = allBookings.Where(b => myIds.Contains(b.Id.Value)).ToList();
        var allLinks = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
        static int CustomerRowCount(IReadOnlyList<BookingCustomer> links, int idBooking) =>
            links.Count(l => l.IdBooking == idBooking);

        var eligible = new List<Booking>();
        foreach (var b in mine.Where(x => x.IdStatus == confirmedId).OrderByDescending(x => x.FlightDate.Value))
        {
            var n = CustomerRowCount(allLinks, b.Id.Value);
            if (n <= 0 || n != b.SeatCount.Value)
                continue;
            var row = await context.Set<BookingEntity>().FirstOrDefaultAsync(e => e.IdBooking == b.Id.Value, ct);
            if (row is null || !row.ConsentDataProcessing || string.IsNullOrWhiteSpace(row.HolderEmail))
                continue;
            eligible.Add(b);
        }

        if (eligible.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay reservas listas para pagar desde aquí.[/]");
            AnsiConsole.MarkupLine(
                "[grey]Hace falta estado «Confirmada», un pasajero por plaza, titular con correo y aceptación de tratamiento de datos. " +
                "Completá esos datos en el flujo de [bold]Buscar vuelos[/] al armar la reserva (antes del pago).[/]");
            return null;
        }

        AnsiConsole.MarkupLine(
            "[cyan]Elegí la reserva a pagar:[/] [grey](los importes son el mismo cálculo que en «Confirmar y pagar» del flujo de reserva)[/]\n");
        var choices = new List<string> { "0 · Volver sin pagar" };
        foreach (var b in eligible)
        {
            var flightNo = flightMap.TryGetValue(b.IdFlight, out var fn) ? fn : b.IdFlight.ToString();
            var raw = await ComputeBookingPayableRawTotalAsync(context, b.Id.Value, ct);
            var oneTotal = decimal.Round(raw, 0, MidpointRounding.AwayFromZero);
            var priceLabel = FareSeatClassPricingHelper.FormatPriceCopColombia(oneTotal);
            choices.Add(
                $"{b.Id.Value} · {b.Code.Value} · Vuelo {flightNo} · {b.FlightDate.Value:yyyy-MM-dd HH:mm} · {priceLabel}");
        }

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Reservas disponibles para pagar:")
                .PageSize(12)
                .AddChoices(choices));

        if (selected.StartsWith("0 ·", StringComparison.Ordinal))
            return null;

        var head = selected.Split(new[] { " · " }, 2, StringSplitOptions.None)[0].Trim();
        if (!int.TryParse(head, NumberStyles.Integer, CultureInfo.InvariantCulture, out var bookingId))
            return null;

        if (eligible.All(b => b.Id.Value != bookingId))
            return null;

        return bookingId;
    }

    private static async Task ClientOpenPaymentFromMyReservationsAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]MIS RESERVAS — PAGO[/]").Centered());
        AnsiConsole.MarkupLine(
            "[grey]Vas a ver el mismo resumen de importes y el mismo flujo de pago que al confirmar una reserva nueva.[/]\n");

        var id = await PromptClientBookingIdEligibleForCheckoutAsync(ct);
        if (id is null)
        {
            AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
            Console.ReadKey();
            return;
        }

        await RunClientCheckoutForBookingsAsync(new[] { id.Value }, ct);
    }

    /// <summary>Checkout cliente: total según tarifa y asientos; valida datos del método; registra pago y marca Pagada.</summary>
    public static async Task RunClientCheckoutForBookingsAsync(int[] bookingIds, CancellationToken ct)
    {
        if (bookingIds.Length == 0 || AppState.IdUserRole == 1)
            return;

        var distinct = bookingIds.Distinct().ToArray();
        if (distinct.Length == 0)
            return;

        Console.Clear();
        AnsiConsole.Write(new Rule("[cyan]CONFIRMAR Y PAGAR[/]").Centered());
        AnsiConsole.MarkupLine(
            "[grey]Total calculado según la tarifa del vuelo y la clase de cada asiento de tu(s) reserva(s).[/]\n");

        try
        {
            var myIds = await GetMyBookingIdsAsync(ct);
            foreach (var id in distinct)
            {
                if (!myIds.Contains(id))
                    throw new InvalidOperationException("Una de las reservas no pertenece a tu cuenta.");
            }

            using var context = DbContextFactory.Create();
            int pagadaId;
            int approvedPaymentId;
            try
            {
                pagadaId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusPaid, ct);
            }
            catch (InvalidOperationException)
            {
                AnsiConsole.MarkupLine(
                    "[red]No existe el estado «Pagada» para reservas. Aplicá la migración AddBookingStatusPagada en la base de datos.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            approvedPaymentId = await SelectStatusByNameAsync(context, PaymentEntityType, PaymentStatusApproved, ct);
            var confirmedId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusConfirmed, ct);
            var canceledId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusCanceled, ct);

            var bookings = new List<Booking>();
            foreach (var id in distinct)
            {
                var b = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(id, ct);
                if (b is null)
                    throw new InvalidOperationException($"No existe la reserva con ID {id}.");
                if (b.IdStatus == canceledId)
                    throw new InvalidOperationException($"La reserva {id} está cancelada; no se puede pagar.");
                bookings.Add(b);
            }

            if (bookings.All(b => b.IdStatus == pagadaId))
            {
                AnsiConsole.MarkupLine("[green]Estas reservas ya figuran como Pagadas; no se duplica el pago.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            if (bookings.Any(b => b.IdStatus == pagadaId))
                throw new InvalidOperationException("Estado inconsistente entre reservas; contactá al administrador.");

            if (bookings.Any(b => b.IdStatus != confirmedId))
                throw new InvalidOperationException("El pago solo aplica a reservas en estado Confirmada (completá pasajeros y datos antes).");

            foreach (var id in distinct)
            {
                var row = await context.Set<BookingEntity>().FirstOrDefaultAsync(b => b.IdBooking == id, ct);
                if (row is null)
                    throw new InvalidOperationException($"No se encontró la reserva con ID {id}.");
                if (!row.ConsentDataProcessing || string.IsNullOrWhiteSpace(row.HolderEmail))
                    throw new InvalidOperationException(
                        "Sin aceptar el tratamiento de datos personales y sin contacto del titular guardado no se puede pagar. " +
                        "Completá titular, correo y aceptación en [bold]Buscar vuelos[/] antes del pago, o cancelá la reserva y volvé a buscar vuelos.");
            }

            var parts = await ComputeAndDisplayClientPaymentSummaryAsync(context, distinct, ct);

            var methodPick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Método de pago:")
                    .PageSize(4)
                    .AddChoices("1. Tarjeta de crédito y débito", "2. PSE"));
            var idPaymentMethod = methodPick.StartsWith("2.", StringComparison.Ordinal) ? 5 : 1;

            if (methodPick.StartsWith("1.", StringComparison.Ordinal))
                PromptMandatoryCardPaymentFields();
            else
                await PromptMandatoryPsePaymentFieldsAsync(ct);

            if (!AnsiConsole.Confirm("\nConfirmar y pagar", true))
            {
                AnsiConsole.MarkupLine("[grey]Pago cancelado. Podés intentarlo de nuevo desde [bold]Buscar vuelos[/] (resumen y pago) o revisá el estado en [bold]Gestión de reservas[/].[/]");
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]");
                Console.ReadKey();
                return;
            }

            var tickets = await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct);
            var paymentRepo = new PaymentRepository(context);
            var bookingRepo = new BookingRepository(context);
            var historyRepo = new BookingStatusHistoryRepository(context);

            for (var i = 0; i < distinct.Length; i++)
            {
                var idB = distinct[i];
                var bk = bookings.First(b => b.Id.Value == idB);
                var idTicket = tickets.FirstOrDefault(t => t.IdBooking == idB)?.Id.Value;
                await new CreatePaymentUseCase(paymentRepo)
                    .ExecuteAsync(parts[i], DateTime.Now, idB, idPaymentMethod, approvedPaymentId, idTicket, ct);

                await new UpdateBookingUseCase(bookingRepo)
                    .ExecuteAsync(
                        bk.Id.Value,
                        bk.Code.Value,
                        bk.FlightDate.Value,
                        bk.CreationDate.Value,
                        bk.SeatCount.Value,
                        bk.Observations.Value,
                        bk.IdFlight,
                        pagadaId,
                        ct);

                await new CreateBookingStatusHistoryUseCase(historyRepo)
                    .ExecuteAsync(
                        DateTime.Now,
                        $"Reserva en estado Pagada (pago registrado, método de pago id {idPaymentMethod}).",
                        idB,
                        pagadaId,
                        AppState.IdUser,
                        ct);
            }

            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Pago registrado[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task<decimal> ComputeBookingPayableRawTotalAsync(AppDbContext context, int bookingId, CancellationToken ct)
    {
        var booking = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(bookingId, ct);
        if (booking is null)
            return 0m;

        var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(booking.IdFlight, ct);
        Fare? fare = null;
        Dictionary<int, decimal>? pricesByClass = null;
        if (flight.IdFare is int fid && fid > 0)
        {
            try
            {
                fare = await new GetFareByIdUseCase(new FareRepository(context)).ExecuteAsync(fid, ct);
            }
            catch
            {
                fare = null;
            }

            if (fare is not null)
            {
                var map = await FareSeatClassPricingHelper.LoadSeatClassPricesByFareIdAsync(context, new[] { fid }, ct);
                if (map.TryGetValue(fid, out var inner) && inner.Count > 0)
                    pricesByClass = inner;
            }
        }

        var links = (await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct))
            .Where(l => l.IdBooking == bookingId)
            .ToList();
        var allSeats = await new GetAllSeatsUseCase(new SeatRepository(context)).ExecuteAsync(ct);
        var seatById = allSeats.ToDictionary(s => s.Id.Value);

        decimal sum = 0m;
        foreach (var link in links)
        {
            if (!seatById.TryGetValue(link.IdSeat, out var seat))
                continue;
            if (fare is null)
                sum += 500_000m;
            else
                sum += FareSeatClassPricingHelper.GetSeatClassTotalPrice(fare, pricesByClass, seat.IdClase);
        }

        if (sum <= 0m && booking.SeatCount.Value > 0)
            sum = booking.SeatCount.Value * (fare?.BasePrice.Value ?? 500_000m);

        return sum;
    }

    /// <summary>Convierte totales por reserva en enteros COP que suman al total redondeado del viaje.</summary>
    private static decimal[] BuildCopPaymentPartsFromRawTotals(IReadOnlyList<decimal> rawPerBooking)
    {
        var n = rawPerBooking.Count;
        if (n == 0)
            return Array.Empty<decimal>();

        var target = decimal.Round(rawPerBooking.Sum(), 0, MidpointRounding.AwayFromZero);
        if (target <= 0m)
            throw new InvalidOperationException("El total calculado no es válido para registrar el pago.");

        var floors = rawPerBooking.Select(r => decimal.Floor(r)).ToArray();
        var needDecimal = target - floors.Sum();
        var need = (int)needDecimal;
        if (needDecimal != need)
            throw new InvalidOperationException("Error interno al calcular el total en pesos.");

        var result = floors.ToArray();
        var order = Enumerable.Range(0, n)
            .OrderByDescending(i => rawPerBooking[i] - decimal.Floor(rawPerBooking[i]))
            .ThenByDescending(i => i)
            .ToArray();
        for (var k = 0; k < need; k++)
            result[order[k % n]] += 1m;

        if (result.Sum() != target)
            throw new InvalidOperationException("Error interno al repartir el total entre reservas.");

        return result;
    }

    private static ValidationResult ValidateCardNumberDigits(string? s)
    {
        var digits = string.IsNullOrWhiteSpace(s) ? string.Empty : new string(s.Where(char.IsDigit).ToArray());
        if (digits.Length is < 13 or > 19)
            return ValidationResult.Error("[red]Ingresá entre 13 y 19 dígitos[/]");
        return ValidationResult.Success();
    }

    private static ValidationResult ValidateCardExpiryMmYy(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return ValidationResult.Error("[red]Obligatorio[/]");
        var m = Regex.Match(input.Trim(), @"^(\d{2})/(\d{2})$");
        if (!m.Success)
            return ValidationResult.Error("[red]Usá el formato MM/AA[/]");
        var mm = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
        var yy = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
        if (mm is < 1 or > 12)
            return ValidationResult.Error("[red]Mes inválido[/]");
        var fullYear = yy <= 50 ? 2000 + yy : 1900 + yy;
        int lastDay;
        try
        {
            lastDay = DateTime.DaysInMonth(fullYear, mm);
        }
        catch
        {
            return ValidationResult.Error("[red]Fecha inválida[/]");
        }

        var lastValidDay = new DateOnly(fullYear, mm, lastDay);
        if (lastValidDay < DateOnly.FromDateTime(DateTime.Today))
            return ValidationResult.Error("[red]Tarjeta vencida[/]");
        return ValidationResult.Success();
    }

    private static ValidationResult ValidateCvv(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return ValidationResult.Error("[red]Obligatorio[/]");
        var t = s.Trim();
        if (!Regex.IsMatch(t, @"^\d{3,4}$"))
            return ValidationResult.Error("[red]3 o 4 dígitos[/]");
        return ValidationResult.Success();
    }

    private static void PromptMandatoryCardPaymentFields()
    {
        AnsiConsole.MarkupLine("[grey]Información de la tarjeta[/]\n");
        _ = AnsiConsole.Prompt(
            new TextPrompt<string>("Número de tarjeta:")
                .Validate(ValidateCardNumberDigits));
        _ = AnsiConsole.Prompt(
            new TextPrompt<string>("Fecha de expiración (MM/AA):")
                .Validate(ValidateCardExpiryMmYy));
        _ = AnsiConsole.Prompt(
            new TextPrompt<string>("CVV:")
                .Validate(ValidateCvv));
    }

    private static async Task PromptMandatoryPsePaymentFieldsAsync(CancellationToken ct)
    {
        AnsiConsole.MarkupLine("[grey]Pago por PSE — información bancaria y titular[/]\n");
        _ = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Banco:")
                .PageSize(12)
                .AddChoices(PseBankChoices));
        _ = AnsiConsole.Prompt(
            new TextPrompt<string>("Nombre del titular de la cuenta:")
                .Validate(s =>
                    !string.IsNullOrWhiteSpace(s) && s.Trim().Length >= 2
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Nombre obligatorio[/]")));
        _ = await SelectDocumentTypeForPassengerFormAsync(ct, "Tipo de documento del titular:");
        _ = AnsiConsole.Prompt(
            new TextPrompt<string>("Número de documento:")
                .Validate(s =>
                    !string.IsNullOrWhiteSpace(s) && s.Trim().Length >= 4
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Número obligatorio[/]")));
    }

    private static string? AppendPassengerPreferenceLines(string? existing, IReadOnlyList<string?> additions)
    {
        var parts = additions.Where(static a => !string.IsNullOrWhiteSpace(a)).Select(static a => a!.Trim()).ToList();
        if (parts.Count == 0)
            return existing;
        var block = string.Join(" | ", parts);
        var c = string.IsNullOrWhiteSpace(existing) ? null : existing.Trim();
        var merged = c is null ? block : $"{c} | {block}";
        return merged.Length <= 500 ? merged : merged[..500];
    }

    private static DateOnly PromptPassengerBirthDateOnly()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var yearMax = today.Year;
        var yearMin = yearMax - 120;
        var yStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Fecha de nacimiento — Año:")
                .PageSize(15)
                .AddChoices(Enumerable.Range(yearMin, yearMax - yearMin + 1).Reverse().Select(i => i.ToString(System.Globalization.CultureInfo.InvariantCulture))));
        var y = int.Parse(yStr, System.Globalization.CultureInfo.InvariantCulture);
        var mStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Fecha de nacimiento — Mes:")
                .PageSize(15)
                .AddChoices(Enumerable.Range(1, 12).Select(i => i.ToString("00", System.Globalization.CultureInfo.InvariantCulture))));
        var m = int.Parse(mStr, System.Globalization.CultureInfo.InvariantCulture);
        var daysInMonth = DateTime.DaysInMonth(y, m);
        var dStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Fecha de nacimiento — Día:")
                .PageSize(15)
                .AddChoices(Enumerable.Range(1, daysInMonth).Select(i => i.ToString(System.Globalization.CultureInfo.InvariantCulture))));
        var d = int.Parse(dStr, System.Globalization.CultureInfo.InvariantCulture);
        return new DateOnly(y, m, d);
    }

    private static async Task<int> SelectDocumentTypeForPassengerFormAsync(CancellationToken ct, string title = "Tipo de documento de viaje:")
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllDocumentTypesUseCase(new DocumentTypeRepository(context)).ExecuteAsync(ct);
        if (!items.Any())
            throw new InvalidOperationException("No hay tipos de documento. Crea uno primero en administración.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title(title).PageSize(12)
                .AddChoices(items.Select(d => $"{d.Id.Value}. {d.Name.Value}")));
        return int.Parse(selected.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);
    }

    private static async Task<int> SelectGenderForPassengerFormAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllGendersUseCase(new GenderRepository(context)).ExecuteAsync(ct);
        if (!items.Any())
            throw new InvalidOperationException("No hay géneros registrados. Crea uno primero en administración.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Género:").PageSize(12)
                .AddChoices(items.Select(g => $"{g.Id.Value}. {g.Description.Value}")));
        return int.Parse(selected.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);
    }

    private static async Task<int> SelectCountryNationalityForPassengerFormAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllCountriesUseCase(new CountryRepository(context)).ExecuteAsync(ct);
        if (!items.Any())
            throw new InvalidOperationException("No hay países registrados. Crea uno primero en administración.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Nacionalidad del documento de viaje:").PageSize(12)
                .AddChoices(items.Select(c => $"{c.Id.Value}. {c.Name.Value} ({c.ISOCode.Value})")));
        return int.Parse(selected.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <param name="boostFlightId">Al editar una reserva, suma temporalmente estos asientos al cupo mostrado del vuelo indicado (los que ya tenía la reserva).</param>
    private static async Task<(int id, DateTime flightDateTime)> SelectFlightAsync(CancellationToken ct, int? boostFlightId = null, int boostSeats = 0)
    {
        using var context = DbContextFactory.Create();
        var flights = await new GetAllFlightsUseCase(new FlightRepository(context)).ExecuteAsync(ct);
        static int EffectiveAvailable(Flight f, int? boostFlightId, int boostSeats) =>
            f.AvailableSeats.Value + (boostFlightId == f.Id.Value ? boostSeats : 0);

        var available = flights.Where(f => EffectiveAvailable(f, boostFlightId, boostSeats) > 0).ToList();
        if (!available.Any()) throw new InvalidOperationException("No hay vuelos con asientos disponibles.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el vuelo:").PageSize(10)
                .AddChoices(available.Select(f =>
                    $"{f.Id.Value}. {f.Number.Value} — {f.Date.Value:yyyy-MM-dd} {f.DepartureTime.Value:HH\\:mm} (disponibles: {EffectiveAvailable(f, boostFlightId, boostSeats)})")));
        var id = int.Parse(selected.Split(new char[] { '.' })[0]);
        var flight = available.First(f => f.Id.Value == id);
        var flightDateTime = flight.Date.Value.ToDateTime(flight.DepartureTime.Value);
        return (id, flightDateTime);
    }

    private static async Task<int> SelectStatusAsync(CancellationToken ct, string entityType = "Booking")
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

    private static async Task<int> SelectDefaultStatusAsync(CancellationToken ct, string entityType = "Booking")
    {
        using var context = DbContextFactory.Create();
        return await SelectStatusByNameAsync(context, entityType, BookingStatusPending, ct);
    }

    private static async Task<int> SelectStatusByNameAsync(src.shared.context.AppDbContext context, string entityType, string statusName, CancellationToken ct)
    {
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var match = statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, entityType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, statusName, StringComparison.OrdinalIgnoreCase));
        if (match is null)
            throw new InvalidOperationException($"No existe el estado '{statusName}' para '{entityType}'. Revisa SystemStatus (Semillas).");
        return match.Id.Value;
    }

    private static string GenerateBookingCode()
    {
        // 6-20 caracteres, solo letras/números, sin guiones.
        // Formato: BK + (IdUser 2-6d) + (random 4d) => ~8-12 chars.
        var rnd = Random.Shared.Next(1000, 9999);
        var code = $"BK{AppState.IdUser}{rnd}";
        if (code.Length < 6) code = code.PadRight(6, '0');
        if (code.Length > 20) code = code.Substring(0, 20);
        return code;
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        if (AppState.IdUserRole != 1)
            throw new InvalidOperationException("Acceso denegado. Los clientes crean reservas desde la búsqueda de vuelos.");

        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR RESERVA[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear una reserva?", true))
            return;
        var code = AnsiConsole.Prompt(
            new TextPrompt<string>("Código de reserva (6-20, solo letras/números, sin guiones). Ej: BK0001:")
                .Validate(v =>
                    SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject.BookingCode
                            .Create(v) is not null
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Código inválido[/]")));
        var seatCount = AnsiConsole.Prompt(
            new TextPrompt<int>("Número de asientos:")
                .Validate(v => v > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Debe ser mayor a 0[/]")));
        var obs = AnsiConsole.Prompt(
                new TextPrompt<string>("Observaciones (Enter para omitir):")
                    .AllowEmpty())
            .Trim();
        string? observations = string.IsNullOrEmpty(obs) ? null : obs;
        try
        {
            using var context = DbContextFactory.Create();
            var (idFlight, flightDateTime) = await SelectFlightAsync(ct);
            var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(idFlight, ct);
            if (seatCount > flight.AvailableSeats.Value)
                throw new InvalidOperationException($"No hay suficientes asientos disponibles. Disponibles: {flight.AvailableSeats.Value}.");

            var idStatus = await SelectStatusAsync(ct);
            await new CreateBookingUseCase(new BookingRepository(context))
                .ExecuteAsync(code, flightDateTime, DateOnly.FromDateTime(DateTime.Today), seatCount, observations, idFlight, idStatus, ct);

            await new UpdateFlightUseCase(new FlightRepository(context))
                .ExecuteAsync(
                    flight.Id.Value,
                    flight.Number.Value,
                    flight.Date.Value,
                    flight.DepartureTime.Value,
                    flight.ArrivalTime.Value,
                    flight.TotalCapacity.Value,
                    flight.AvailableSeats.Value - seatCount,
                    flight.IdRoute,
                    flight.IdAircraft,
                    flight.IdStatus,
                    flight.IdCrew,
                    flight.IdFare,
                    flight.BoardingGate,
                    ct);

            await context.SaveChangesAsync(ct);
            var bookingRepo = new BookingRepository(context);
            var codeKey = code.Trim().ToUpperInvariant();
            var saved = await bookingRepo.GetByCodeAsync(codeKey, ct);
            if (saved is not null)
            {
                try
                {
                    var emitC = await EmitTicketForNewBookingAsync(context, flight, saved.Id.Value, seatCount, ct);
                    if (emitC is { } ec)
                    {
                        AnsiConsole.MarkupLine(
                            $"\n[green]Reserva '[bold]{Markup.Escape(saved.Code.Value)}[/]' creada con ID {saved.Id.Value}. " +
                            $"Tiquete [bold]{Markup.Escape(ec.TicketCode)}[/] (ID {ec.TicketId}). " +
                            "[grey]El pago se registra aparte cuando corresponda (menú de pagos).[/][/]");
                    }
                    else
                        AnsiConsole.MarkupLine($"\n[green]Reserva '[bold]{Markup.Escape(saved.Code.Value)}[/]' creada con ID {saved.Id.Value}.[/]");
                }
                catch (InvalidOperationException ex)
                {
                    AnsiConsole.MarkupLine($"\n[green]Reserva '[bold]{Markup.Escape(saved.Code.Value)}[/]' creada con ID {saved.Id.Value}.[/]");
                    AnsiConsole.MarkupLine($"[yellow]Aviso: no se emitió tiquete automático — {Markup.Escape(ex.Message)}[/]");
                }
            }
            else
                AnsiConsole.MarkupLine($"\n[green]Reserva registrada correctamente (código [bold]{Markup.Escape(codeKey)}[/]).[/]");
            if (saved is not null && AnsiConsole.Confirm("\n¿Agregar pasajero a esta reserva ahora?", true))
                await AddPassengerAsync(ct, saved.Id.Value);
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR RESERVA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la reserva a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var code = AnsiConsole.Ask<string>("Nuevo código:");
        var seatCount = AnsiConsole.Ask<int>("Número de asientos:");
        var obs = AnsiConsole.Prompt(
                new TextPrompt<string>("Observaciones (Enter para omitir):")
                    .AllowEmpty())
            .Trim();
        string? observations = string.IsNullOrEmpty(obs) ? null : obs;
        try
        {
            using var context = DbContextFactory.Create();
            var oldBooking = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(id, ct);
            var canceledId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusCanceled, ct);
            bool HoldsSeats(int statusId) => statusId != canceledId;

            var boostSeats = HoldsSeats(oldBooking.IdStatus) ? oldBooking.SeatCount.Value : 0;
            var (idFlight, flightDateTime) = await SelectFlightAsync(ct, boostFlightId: oldBooking.IdFlight, boostSeats: boostSeats);
            var idStatus = await SelectStatusAsync(ct);

            var holdsOld = HoldsSeats(oldBooking.IdStatus);
            var holdsNew = HoldsSeats(idStatus);

            var oldFlight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(oldBooking.IdFlight, ct);
            var newFlight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(idFlight, ct);

            if (oldBooking.IdFlight == idFlight)
            {
                var pool = newFlight.AvailableSeats.Value;
                if (holdsOld) pool += oldBooking.SeatCount.Value;
                if (holdsNew)
                {
                    if (pool < seatCount)
                        throw new InvalidOperationException($"No hay suficientes asientos en el vuelo. Efectivos tras liberar la reserva: {pool}.");
                    pool -= seatCount;
                }

                await new UpdateFlightUseCase(new FlightRepository(context))
                    .ExecuteAsync(
                        newFlight.Id.Value,
                        newFlight.Number.Value,
                        newFlight.Date.Value,
                        newFlight.DepartureTime.Value,
                        newFlight.ArrivalTime.Value,
                        newFlight.TotalCapacity.Value,
                        pool,
                        newFlight.IdRoute,
                        newFlight.IdAircraft,
                        newFlight.IdStatus,
                        newFlight.IdCrew,
                        newFlight.IdFare,
                        newFlight.BoardingGate,
                        ct);
            }
            else
            {
                if (holdsOld)
                {
                    await new UpdateFlightUseCase(new FlightRepository(context))
                        .ExecuteAsync(
                            oldFlight.Id.Value,
                            oldFlight.Number.Value,
                            oldFlight.Date.Value,
                            oldFlight.DepartureTime.Value,
                            oldFlight.ArrivalTime.Value,
                            oldFlight.TotalCapacity.Value,
                            oldFlight.AvailableSeats.Value + oldBooking.SeatCount.Value,
                            oldFlight.IdRoute,
                            oldFlight.IdAircraft,
                            oldFlight.IdStatus,
                            oldFlight.IdCrew,
                            oldFlight.IdFare,
                            oldFlight.BoardingGate,
                            ct);
                }

                newFlight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(idFlight, ct);
                if (holdsNew)
                {
                    if (newFlight.AvailableSeats.Value < seatCount)
                        throw new InvalidOperationException($"No hay suficientes asientos en el vuelo destino. Disponibles: {newFlight.AvailableSeats.Value}.");
                    await new UpdateFlightUseCase(new FlightRepository(context))
                        .ExecuteAsync(
                            newFlight.Id.Value,
                            newFlight.Number.Value,
                            newFlight.Date.Value,
                            newFlight.DepartureTime.Value,
                            newFlight.ArrivalTime.Value,
                            newFlight.TotalCapacity.Value,
                            newFlight.AvailableSeats.Value - seatCount,
                            newFlight.IdRoute,
                            newFlight.IdAircraft,
                            newFlight.IdStatus,
                            newFlight.IdCrew,
                            newFlight.IdFare,
                            newFlight.BoardingGate,
                            ct);
                }
            }

            await new UpdateBookingUseCase(new BookingRepository(context))
                .ExecuteAsync(id, code, flightDateTime, oldBooking.CreationDate.Value, seatCount, observations, idFlight, idStatus, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Reserva actualizada correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    /// <summary>Cliente: solo búsqueda por código PNR (no por ID numérico).</summary>
    private static async Task<int> ResolveBookingIdFromPnrOnlyAsync(BookingRepository bookingRepo, string trimmed, CancellationToken ct)
    {
        var codeKey = trimmed.ToUpperInvariant();
        var byCode = await bookingRepo.GetByCodeAsync(codeKey, ct);
        if (byCode is null)
            throw new InvalidOperationException(
                "No se encontró una reserva con ese código (PNR). Copiá el código tal como aparece en «Ver mis reservas» (ej. BK56009).");

        return byCode.Id.Value;
    }

    /// <summary>Administrador: PNR o ID numérico de reserva.</summary>
    private static async Task<int> ResolveBookingIdFromCancelInputAsync(
        BookingRepository bookingRepo,
        string trimmed,
        CancellationToken ct)
    {
        if (int.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out var asInt) && asInt > 0)
        {
            var byId = await bookingRepo.GetByIdAsync(BookingId.Create(asInt), ct);
            if (byId is not null)
                return byId.Id.Value;
        }

        var codeKey = trimmed.ToUpperInvariant();
        var byCode = await bookingRepo.GetByCodeAsync(codeKey, ct);
        if (byCode is not null)
            return byCode.Id.Value;

        throw new InvalidOperationException(
            "No se encontró una reserva con ese código o ID. Verificá el código (ej. BK56009) o el número de reserva.");
    }

    private static async Task CancelAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CANCELAR RESERVA[/]").Centered());
        if (AppState.IdUserRole != 1)
        {
            AnsiConsole.MarkupLine(
                "[grey]Solo podés cancelar con el [bold]código de reserva (PNR)[/], igual que en [bold]Ver mis reservas[/] (ej. BK56009).[/]\n");
        }

        var promptLabel = AppState.IdUserRole == 1
            ? "Código de reserva (PNR) o ID numérico (0 = Volver):"
            : "Código de reserva (PNR) (0 = Volver):";
        var rawInput = AnsiConsole.Prompt(new TextPrompt<string>(promptLabel).AllowEmpty());
        var trimmedInput = (rawInput ?? string.Empty).Trim();
        if (trimmedInput.Length == 0 || trimmedInput == "0")
            return;

        const string defaultCancelReason = "Cancelación solicitada por el pasajero (cliente)";
        var reasonInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Motivo de cancelación (mín. 5 caracteres; Enter = texto por defecto):")
                .AllowEmpty());
        var reason = string.IsNullOrWhiteSpace(reasonInput) ? defaultCancelReason : reasonInput.Trim();
        if (reason.Length < 5)
            reason = defaultCancelReason;

        var penalty = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Monto de penalización (0 si no aplica):")
                .DefaultValue(0m));
        try
        {
            using var context = DbContextFactory.Create();
            var bookingRepo = new BookingRepository(context);
            var idBooking = AppState.IdUserRole == 1
                ? await ResolveBookingIdFromCancelInputAsync(bookingRepo, trimmedInput, ct)
                : await ResolveBookingIdFromPnrOnlyAsync(bookingRepo, trimmedInput, ct);

            if (AppState.IdUserRole != 1)
            {
                var myBookingIds = await GetMyBookingIdsAsync(ct);
                if (!myBookingIds.Contains(idBooking))
                    throw new InvalidOperationException("No puedes cancelar una reserva que no te pertenece.");
            }

            // Recuperar reserva y vuelo para restaurar disponibilidad.
            var booking = await new GetBookingByIdUseCase(bookingRepo).ExecuteAsync(idBooking, ct);
            var canceledStatusId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusCanceled, ct);
            if (booking.IdStatus == canceledStatusId)
            {
                AnsiConsole.MarkupLine("\n[yellow]Esta reserva ya está cancelada.[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
                return;
            }

            var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(booking.IdFlight, ct);

            await new CreateBookingCancellationUseCase(new BookingCancellationRepository(context))
                .ExecuteAsync(DateTime.Now, reason, penalty, idBooking, AppState.IdUser, ct);

            // Marcar reserva como cancelada.
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
                    flight.BoardingGate,
                    ct);

            // Liberar asientos asignados en la reserva (SeatFlight.Available = true).
            await ReleaseSeatFlightsForBookingAsync(context, booking.Id.Value, booking.IdFlight, ct);

            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Cancelación registrada correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task AddPassengerAsync(CancellationToken ct, int? prefilledBookingId = null)
    {
        if (AppState.IdUserRole != 1)
            throw new InvalidOperationException("Acceso denegado.");

        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]AGREGAR PASAJERO A RESERVA[/]").Centered());
        var idBooking = prefilledBookingId ?? AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la reserva (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (idBooking == 0) return;
        try
        {
            using var ctx = DbContextFactory.Create();

            // Filtrar asientos disponibles del vuelo de esta reserva
            var bookings = await new GetAllBookingsUseCase(new BookingRepository(ctx)).ExecuteAsync(ct);
            var booking = bookings.FirstOrDefault(b => b.Id.Value == idBooking);
            if (booking is null) throw new InvalidOperationException($"No se encontró la reserva con ID {idBooking}.");

            var seatFlights = await new GetAllSeatFlightsUseCase(new SeatFlightRepository(ctx)).ExecuteAsync(ct);
            var availableSeatIds = seatFlights
                .Where(sf => sf.IdFlight == booking.IdFlight && sf.Available)
                .Select(sf => sf.IdSeat)
                .ToHashSet();

            var allSeats = await new GetAllSeatsUseCase(new SeatRepository(ctx)).ExecuteAsync(ct);
            var flightSeats = allSeats.Where(s => availableSeatIds.Contains(s.Id.Value)).ToList();
            if (!flightSeats.Any()) throw new InvalidOperationException("No hay asientos disponibles en el vuelo de esta reserva.");

            var persons = await new GetAllPersonsUseCase(new PersonRepository(ctx)).ExecuteAsync(ct);
            if (!persons.Any()) throw new InvalidOperationException("No hay personas registradas.");
            var selPerson = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Selecciona la persona:").PageSize(10)
                    .AddChoices(persons.Select(p => $"{p.Id.Value}. {p.FirstName.Value} {p.LastName.Value}")));
            var idPerson = int.Parse(selPerson.Split(new char[] { '.' })[0]);

            AnsiConsole.MarkupLine($"[grey]Vuelo: {booking.IdFlight} — {flightSeats.Count} asiento(s) disponible(s)[/]");
            var selSeat = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Selecciona el asiento:").PageSize(10)
                    .AddChoices(flightSeats.Select(s => $"{s.Id.Value}. {s.Number.Value}")));
            var idSeat = int.Parse(selSeat.Split(new char[] { '.' })[0]);

            var isPrimary = AnsiConsole.Confirm("¿Es el pasajero titular de la reserva?", false);
            using var context = DbContextFactory.Create();
            await new CreateBookingCustomerUseCase(new BookingCustomerRepository(context))
                .ExecuteAsync(DateTime.Now, idBooking, AppState.IdUser, idPerson, idSeat, isPrimary, ct);

            // Bloquear el asiento seleccionado para este vuelo.
            await SetSeatFlightAvailabilityAsync(context, idSeat, booking.IdFlight, available: false, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Pasajero agregado a la reserva correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static string FormatSeatLegLongForUi(string? flightLegLabel)
    {
        if (string.IsNullOrWhiteSpace(flightLegLabel))
            return "este vuelo";
        return flightLegLabel.Trim().ToUpperInvariant() switch
        {
            "IDA" => "vuelo de IDA (primer tramo, ida)",
            "VUELTA" => "vuelo de VUELTA (regreso)",
            _ => flightLegLabel.Trim(),
        };
    }

    private static string FormatSeatLegShortTag(string? flightLegLabel)
    {
        if (string.IsNullOrWhiteSpace(flightLegLabel))
            return "Este vuelo";
        return flightLegLabel.Trim().ToUpperInvariant() switch
        {
            "IDA" => "IDA — ida",
            "VUELTA" => "VUELTA — regreso",
            _ => flightLegLabel.Trim(),
        };
    }

    private static string FormatSeatLegSuccessSuffix(string? flightLegLabel)
    {
        if (string.IsNullOrWhiteSpace(flightLegLabel))
            return " en el vuelo";
        return flightLegLabel.Trim().ToUpperInvariant() switch
        {
            "IDA" => " en el vuelo de IDA",
            "VUELTA" => " en el vuelo de VUELTA",
            _ => " en el vuelo",
        };
    }

    private static async Task<int> ResolveFareIdForClientTicketAsync(AppDbContext context, Flight flight, CancellationToken ct)
    {
        if (flight.IdFare is int fId && fId > 0)
            return fId;

        var aircraft = await new GetAircraftByIdUseCase(new AircraftRepository(context)).ExecuteAsync(flight.IdAircraft, ct);

        var fares = (await new GetAllFaresUseCase(new FareRepository(context)).ExecuteAsync(ct))
            .Where(f => f.Active && f.IdAirline == aircraft.IdAirline)
            .OrderBy(f => f.Id.Value)
            .ToList();
        if (fares.Count == 0)
            throw new InvalidOperationException(
                "El vuelo no tiene tarifa asignada y no hay tarifa activa para la aerolínea. Asigná una tarifa al vuelo o creá un tarifario.");

        return fares[0].Id.Value;
    }

    private static string NewClientTicketCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var buf = new char[10];
        for (var i = 0; i < 10; i++)
            buf[i] = chars[Random.Shared.Next(chars.Length)];

        return "TK" + new string(buf);
    }

    /// <summary>Emite el tiquete asociado a la reserva (sin registrar pago: eso se hace en el flujo de pagos).</summary>
    private static async Task<(string TicketCode, int TicketId)?> EmitTicketForNewBookingAsync(
        AppDbContext context, Flight flight, int idBooking, int seatCount, CancellationToken ct)
    {
        if (seatCount < 1)
            return null;

        var idFare = await ResolveFareIdForClientTicketAsync(context, flight, ct);
        var idStatusTicket = await SelectStatusByNameAsync(context, TicketEntityType, TicketStatusActive, ct);
        var ticketRepo = new TicketRepository(context);
        var uc = new CreateTicketUseCase(ticketRepo);
        Ticket? issued = null;
        for (var attempt = 0; attempt < 20; attempt++)
        {
            var code = NewClientTicketCode();
            try
            {
                issued = await uc.ExecuteAsync(code, idBooking, idFare, idStatusTicket, ct);
                break;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            {
            }
        }

        if (issued is null)
            throw new InvalidOperationException("No se pudo generar un código de tiquete único. Reintentá.");

        await context.SaveChangesAsync(ct);
        var tDisplay = await ticketRepo.GetByCodeAsync(issued.Code.Value, ct)
            ?? throw new InvalidOperationException("El tiquete no pudo recuperarse tras guardar.");

        var bookingForObs = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(idBooking, ct);
        await BookingPaidBundleBaggageProvisioner.TryProvisionIfNoBaggageAsync(
            context,
            tDisplay.Id.Value,
            bookingForObs?.Observations.Value,
            seatCount,
            ct);
        await context.SaveChangesAsync(ct);

        return (tDisplay.Code.Value, tDisplay.Id.Value);
    }

    private static async Task RemoveIssuedTicketsAndPaymentsForBookingAsync(AppDbContext context, int idBooking, CancellationToken ct)
    {
        var paymentRepo = new PaymentRepository(context);
        var payments = (await new GetAllPaymentsUseCase(paymentRepo).ExecuteAsync(ct))
            .Where(p => p.IdBooking == idBooking)
            .ToList();
        foreach (var p in payments)
            await new DeletePaymentUseCase(paymentRepo).ExecuteAsync(p.Id.Value, ct);

        var ticketRepo = new TicketRepository(context);
        var tickets = (await new GetAllTicketsUseCase(ticketRepo).ExecuteAsync(ct))
            .Where(t => t.IdBooking == idBooking)
            .ToList();
        if (tickets.Count == 0)
            return;

        var ticketIds = new HashSet<int>(tickets.Select(t => t.Id.Value));
        var baggageRepo = new BaggageRepository(context);
        foreach (var b in (await new GetAllBaggagesUseCase(baggageRepo).ExecuteAsync(ct)).Where(b => ticketIds.Contains(b.IdTicket)))
            await new DeleteBaggageUseCase(baggageRepo).ExecuteAsync(b.Id.Value, ct);

        var checkInRepo = new CheckInRepository(context);
        foreach (var c in (await new GetAllCheckInsUseCase(checkInRepo).ExecuteAsync(ct)).Where(c => ticketIds.Contains(c.IdTicket)))
            await new DeleteCheckInUseCase(checkInRepo).ExecuteAsync(c.Id.Value, ct);

        var tshRepo = new TicketStatusHistoryRepository(context);
        foreach (var h in (await new GetAllTicketStatusHistoriesUseCase(tshRepo).ExecuteAsync(ct)).Where(h => ticketIds.Contains(h.IdTicket)))
            await new DeleteTicketStatusHistoryUseCase(tshRepo).ExecuteAsync(h.Id.Value, ct);

        foreach (var t in tickets)
            await new DeleteTicketUseCase(ticketRepo).ExecuteAsync(t.Id.Value, ct);
    }

    private static string BuildClientSeatChoiceLabel(
        Seat s,
        IReadOnlyDictionary<int, string> classNames,
        Fare? fare,
        IReadOnlyDictionary<int, decimal>? pricesByClass,
        decimal referenceEconomyPrice)
    {
        var cn = classNames.TryGetValue(s.IdClase, out var name) ? name : $"Clase {s.IdClase}";
        if (fare is null || pricesByClass is null || pricesByClass.Count == 0)
            return $"{s.Id.Value}. {s.Number.Value} · {cn}";

        var total = FareSeatClassPricingHelper.GetSeatClassTotalPrice(fare, pricesByClass, s.IdClase);
        var add = Math.Max(0m, decimal.Round(total - referenceEconomyPrice, 0, MidpointRounding.AwayFromZero));
        var addTxt = add <= 0
            ? "sin adicional"
            : $"+{FareSeatClassPricingHelper.FormatPriceCopColombia(add)} adicional";
        return $"{s.Id.Value}. {s.Number.Value} · {cn} · {FareSeatClassPricingHelper.FormatPriceCopColombia(total)} ({addTxt})";
    }

    /// <summary>
    /// Tras guardar la reserva del cliente: bloquea en el mapa un asiento por plaza reservada. El titular de cuenta queda como vínculo técnico hasta completar datos (en este flujo o en Mis reservas).
    /// </summary>
    private static async Task AddPassengersAndSeatsForClientBookingAsync(
        AppDbContext context,
        CancellationToken ct,
        int bookingId,
        int seatCount,
        string? seatSelectionFlightLegLabel = null,
        bool deferPassengerDetailsCompletion = false)
    {
        if (AppState.IdPerson is null)
            throw new InvalidOperationException("Tu cuenta no tiene un perfil personal asociado. Contacta al administrador.");
        if (seatCount < 1)
            return;

        var idTitular = AppState.IdPerson.Value;
        var legLong = FormatSeatLegLongForUi(seatSelectionFlightLegLabel);
        var legTag = FormatSeatLegShortTag(seatSelectionFlightLegLabel);

        var passengerHint = deferPassengerDetailsCompletion
            ? "Una vez cerrados [bold]ida[/] y [bold]vuelta[/], te pediremos los datos personales de cada pasajero antes del resumen del viaje."
            : "Después de elegir asientos vas a ingresar los datos personales de cada pasajero (nombre, documento, etc.) en este mismo flujo.";
        AnsiConsole.MarkupLine(
            $"\n[cyan]Asientos[/] [bold]({Markup.Escape(legLong)})[/]\n" +
            $"[grey]Elegí[/] [bold]{seatCount}[/] [grey]asiento(s) distintos en este tramo[/] [bold]{Markup.Escape(legTag)}[/][grey]. " +
            $"Un asiento por plaza de la reserva. {passengerHint}[/]");

        var bookings = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        var booking = bookings.FirstOrDefault(b => b.Id.Value == bookingId)
            ?? throw new InvalidOperationException($"No se encontró la reserva con ID {bookingId}.");

        var seatClasses = await new GetAllSeatClassesUseCase(new SeatClassRepository(context)).ExecuteAsync(ct);
        var classNames = seatClasses.ToDictionary(sc => sc.Id.Value, sc => sc.Name.Value);

        Fare? fareForPricing = null;
        Dictionary<int, decimal>? pricesByClass = null;
        decimal referenceEconomyPrice = 0m;
        try
        {
            var fl = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(booking.IdFlight, ct);
            if (fl.IdFare is int fid && fid > 0)
            {
                fareForPricing = await new GetFareByIdUseCase(new FareRepository(context)).ExecuteAsync(fid, ct);
                var byFareAll = await FareSeatClassPricingHelper.LoadSeatClassPricesByFareIdAsync(context, new[] { fid }, ct);
                if (byFareAll.TryGetValue(fid, out var inner) && inner.Count > 0)
                    pricesByClass = inner;
                referenceEconomyPrice = FareSeatClassPricingHelper.GetReferenceEconomyPrice(
                    fareForPricing, pricesByClass, classNames);
            }
        }
        catch (KeyNotFoundException)
        {
            fareForPricing = null;
            pricesByClass = null;
        }

        if (fareForPricing is not null && pricesByClass is { Count: > 0 })
        {
            var desdeTxt = FareSeatClassPricingHelper.FormatPriceCopColombia(referenceEconomyPrice);
            AnsiConsole.MarkupLine(
                "[grey]Precios de la tarifa del vuelo (como al crearla): [bold]desde[/] " +
                $"{Markup.Escape(desdeTxt)}[/] [grey]— referencia económica / menor precio. En cada asiento: total y adicional.[/]");
        }

        for (var index = 0; index < seatCount; index++)
        {
            var seatFlights = await new GetAllSeatFlightsUseCase(new SeatFlightRepository(context)).ExecuteAsync(ct);
            var availableSeatIds = seatFlights
                .Where(sf => sf.IdFlight == booking.IdFlight && sf.Available)
                .Select(sf => sf.IdSeat)
                .ToHashSet();

            var allSeats = await new GetAllSeatsUseCase(new SeatRepository(context)).ExecuteAsync(ct);
            var flightSeats = allSeats
                .Where(s => availableSeatIds.Contains(s.Id.Value))
                .OrderBy(s => s.Number.Value, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (flightSeats.Count == 0)
            {
                throw new InvalidOperationException(
                    index == 0
                        ? $"No hay asientos disponibles en el mapa del {legLong}. Un administrador puede generarlos desde Aeronaves / Vuelos."
                        : $"Solo se eligieron {index} de {seatCount} asiento(s) en el {legLong}: no quedan más libres. Contactá al administrador para ajustar la reserva.");
            }

            AnsiConsole.MarkupLine(
                $"[bold]{Markup.Escape(legTag)}[/][grey] · Asiento [bold]{index + 1}[/] de [bold]{seatCount}[/] — {flightSeats.Count} libre(s) en el mapa.[/]");

            var selSeat = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"{legTag} — Asiento {index + 1} de {seatCount}: elegí uno libre (distinto a los ya elegidos en este tramo):")
                    .PageSize(15)
                    .AddChoices(flightSeats.Select(s =>
                        BuildClientSeatChoiceLabel(s, classNames, fareForPricing, pricesByClass, referenceEconomyPrice))));
            var idSeat = int.Parse(selSeat.Split(new char[] { '.' })[0], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

            if (fareForPricing is not null && pricesByClass is { Count: > 0 })
            {
                var chosen = flightSeats.First(s => s.Id.Value == idSeat);
                var totalP = FareSeatClassPricingHelper.GetSeatClassTotalPrice(fareForPricing, pricesByClass, chosen.IdClase);
                var add = Math.Max(0m, decimal.Round(totalP - referenceEconomyPrice, 0, MidpointRounding.AwayFromZero));
                var addMarkup = add <= 0
                    ? "[green]sin adicional[/] [grey](referencia económica)[/]"
                    : $"[yellow]{Markup.Escape(FareSeatClassPricingHelper.FormatPriceCopColombia(add))}[/] [grey]adicional[/]";
                AnsiConsole.MarkupLine(
                    $"[grey]Plaza {index + 1}: [bold]{Markup.Escape(FareSeatClassPricingHelper.FormatPriceCopColombia(totalP))}[/] total · {addMarkup}[/]");
            }

            await new CreateBookingCustomerUseCase(new BookingCustomerRepository(context))
                .ExecuteAsync(DateTime.Now, bookingId, AppState.IdUser, idTitular, idSeat, isPrimary: index == 0, ct);

            await SetSeatFlightAvailabilityAsync(context, idSeat, booking.IdFlight, available: false, ct);
            await context.SaveChangesAsync(ct);
        }
    }

    private static async Task SetSeatFlightAvailabilityAsync(src.shared.context.AppDbContext context, int idSeat, int idFlight, bool available, CancellationToken ct)
    {
        var repo = new SeatFlightRepository(context);
        var seatFlight = await repo.GetBySeatAndFlightAsync(idSeat, idFlight, ct);
        if (seatFlight is null)
            throw new InvalidOperationException("El asiento seleccionado no está asociado a este vuelo (SeatFlight no existe). Un administrador debe generar asientos del vuelo.");
        if (seatFlight.Available == available) return;
        await new UpdateSeatFlightUseCase(repo).ExecuteAsync(seatFlight.Id.Value, seatFlight.IdSeat, seatFlight.IdFlight, available, ct);
    }

    private static async Task ReleaseSeatFlightsForBookingAsync(src.shared.context.AppDbContext context, int idBooking, int idFlight, CancellationToken ct)
    {
        var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
        var seatIds = links.Where(l => l.IdBooking == idBooking).Select(l => l.IdSeat).Distinct().ToList();
        if (seatIds.Count == 0) return;
        foreach (var idSeat in seatIds)
            await SetSeatFlightAvailabilityAsync(context, idSeat, idFlight, available: true, ct);
    }

    private static async Task AddStatusHistoryAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CAMBIO DE ESTADO DE RESERVA[/]").Centered());
        var idBooking = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la reserva (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (idBooking == 0) return;
        var obs = AnsiConsole.Prompt(
                new TextPrompt<string>("Observación (Enter para omitir):")
                    .AllowEmpty())
            .Trim();
        string? observation = string.IsNullOrEmpty(obs) ? null : obs;
        try
        {
            var idStatus = await SelectStatusAsync(ct);
            using var context = DbContextFactory.Create();
            var booking = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(idBooking, ct);
            if (booking is null)
                throw new InvalidOperationException($"No existe la reserva con ID {idBooking}.");

            var canceledId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusCanceled, ct);
            bool HoldsSeats(int statusId) => statusId != canceledId;
            var hadSeats = HoldsSeats(booking.IdStatus);
            var willHoldSeats = HoldsSeats(idStatus);

            if (hadSeats && !willHoldSeats)
            {
                var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(booking.IdFlight, ct);
                var newAvail = flight.AvailableSeats.Value + booking.SeatCount.Value;
                if (newAvail > flight.TotalCapacity.Value)
                    throw new InvalidOperationException("El cambio de estado no puede aplicarse: disponibilidad inconsistente.");
                await new UpdateFlightUseCase(new FlightRepository(context))
                    .ExecuteAsync(
                        flight.Id.Value,
                        flight.Number.Value,
                        flight.Date.Value,
                        flight.DepartureTime.Value,
                        flight.ArrivalTime.Value,
                        flight.TotalCapacity.Value,
                        newAvail,
                        flight.IdRoute,
                        flight.IdAircraft,
                        flight.IdStatus,
                        flight.IdCrew,
                        flight.IdFare,
                        flight.BoardingGate,
                        ct);
            }
            else if (!hadSeats && willHoldSeats)
            {
                var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(booking.IdFlight, ct);
                if (flight.AvailableSeats.Value < booking.SeatCount.Value)
                    throw new InvalidOperationException($"No hay suficientes asientos para reactivar la reserva. Disponibles: {flight.AvailableSeats.Value}.");
                await new UpdateFlightUseCase(new FlightRepository(context))
                    .ExecuteAsync(
                        flight.Id.Value,
                        flight.Number.Value,
                        flight.Date.Value,
                        flight.DepartureTime.Value,
                        flight.ArrivalTime.Value,
                        flight.TotalCapacity.Value,
                        flight.AvailableSeats.Value - booking.SeatCount.Value,
                        flight.IdRoute,
                        flight.IdAircraft,
                        flight.IdStatus,
                        flight.IdCrew,
                        flight.IdFare,
                        flight.BoardingGate,
                        ct);
            }

            await new CreateBookingStatusHistoryUseCase(new BookingStatusHistoryRepository(context))
                .ExecuteAsync(DateTime.Now, observation, idBooking, idStatus, AppState.IdUser, ct);

            await new UpdateBookingUseCase(new BookingRepository(context))
                .ExecuteAsync(
                    booking.Id.Value,
                    booking.Code.Value,
                    booking.FlightDate.Value,
                    booking.CreationDate.Value,
                    booking.SeatCount.Value,
                    booking.Observations.Value,
                    booking.IdFlight,
                    idStatus,
                    ct);

            if (!willHoldSeats && hadSeats)
                await ReleaseSeatFlightsForBookingAsync(context, booking.Id.Value, booking.IdFlight, ct);

            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Estado de reserva actualizado (historial + cupos del vuelo).[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR RESERVA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la reserva a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar la reserva con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var booking = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(id, ct);
            if (booking is null)
            {
                AnsiConsole.MarkupLine("\n[yellow]No se encontró la reserva con ese ID.[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
                return;
            }

            await RemoveIssuedTicketsAndPaymentsForBookingAsync(context, id, ct);

            var canceledId = await SelectStatusByNameAsync(context, BookingEntityType, BookingStatusCanceled, ct);
            var holdsSeats = booking.IdStatus != canceledId;

            if (holdsSeats)
            {
                var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(booking.IdFlight, ct);
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
                        flight.BoardingGate,
                        ct);
            }

            await ReleaseSeatFlightsForBookingAsync(context, booking.Id.Value, booking.IdFlight, ct);

            var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
            foreach (var link in links.Where(l => l.IdBooking == id))
                await new DeleteBookingCustomerUseCase(new BookingCustomerRepository(context)).ExecuteAsync(link.Id.Value, ct);

            var histories = await new GetAllBookingStatusHistoriesUseCase(new BookingStatusHistoryRepository(context)).ExecuteAsync(ct);
            foreach (var hist in histories.Where(x => x.IdBooking == id))
                await new DeleteBookingStatusHistoryUseCase(new BookingStatusHistoryRepository(context)).ExecuteAsync(hist.Id.Value, ct);

            var cancellations = await new GetAllBookingCancellationsUseCase(new BookingCancellationRepository(context)).ExecuteAsync(ct);
            foreach (var cancel in cancellations.Where(c => c.IdBooking == id))
                await new DeleteBookingCancellationUseCase(new BookingCancellationRepository(context)).ExecuteAsync(cancel.Id.Value, ct);

            var deleted = await new DeleteBookingUseCase(new BookingRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Reserva eliminada correctamente.[/]" : "\n[yellow]No se pudo eliminar la reserva.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
