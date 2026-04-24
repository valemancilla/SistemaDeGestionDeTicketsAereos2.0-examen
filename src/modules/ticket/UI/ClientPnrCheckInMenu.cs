// Flujo de check-in orientado a aerolínea: PNR + apellido, validación de pago, tramos ida/vuelta, asiento, equipaje y pase de abordar.
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
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Repositories;
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
using CheckInAgg = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.UI;

public static class ClientPnrCheckInMenu
{
    private const string BookingEntityType = "Booking";
    private const string CheckInEntityType = "CheckIn";
    private const string BookingStatusPaid = "Pagada";
    private const string CheckInStatusCompleted = "Completado";

    public static async Task RunAsync(CancellationToken ct = default)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]CHECK-IN — CÓDIGO DE RESERVA (PNR) Y APELLIDO[/]").Centered());
        AnsiConsole.MarkupLine(
            "[grey]Ingresá el PNR y el apellido tal como figuran en la reserva. " +
            "Si tu viaje es [bold]ida y vuelta[/], podés ingresar el PNR de [bold]cualquiera de los dos tramos[/]: " +
            "el sistema agrupa ambas reservas (misma fecha de compra, mismas plazas y mismo pasajero) y te muestra el viaje completo.[/]\n");

        string pnr = AnsiConsole.Prompt(
            new TextPrompt<string>("Código de reserva (PNR):")
                .Validate(v =>
                    string.IsNullOrWhiteSpace(v)
                        ? ValidationResult.Error("[red]El código no puede estar vacío.[/]")
                        : ValidationResult.Success()));

        string surname = AnsiConsole.Prompt(
            new TextPrompt<string>("Apellido del pasajero:")
                .Validate(v =>
                    string.IsNullOrWhiteSpace(v)
                        ? ValidationResult.Error("[red]El apellido no puede estar vacío.[/]")
                        : ValidationResult.Success()));

        var pnrKey = pnr.Trim().ToUpperInvariant();
        var surnameKey = surname.Trim();

        try
        {
            using var context = DbContextFactory.Create();
            var paidId = await ResolveStatusIdAsync(context, BookingEntityType, BookingStatusPaid, ct);
            var checkInCompletedId = await ResolveStatusIdAsync(context, CheckInEntityType, CheckInStatusCompleted, ct);
            var allCheckIns = await new GetAllCheckInsUseCase(new CheckInRepository(context)).ExecuteAsync(ct);
            var allTickets = await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct);

            var bookingRepo = new BookingRepository(context);
            var booking0 = await bookingRepo.GetByCodeAsync(pnrKey, ct);
            if (booking0 is null)
            {
                AnsiConsole.MarkupLine("[red]No existe una reserva con ese código. Verificá el PNR e intentá de nuevo.[/]");
                Pause();
                return;
            }

            if (!await UserMayAccessBookingAsync(context, booking0.Id.Value, ct))
            {
                AnsiConsole.MarkupLine("[red]Esta reserva no está asociada a tu cuenta.[/]");
                Pause();
                return;
            }

            var allLinks = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
            var linksOnPnr = allLinks.Where(l => l.IdBooking == booking0.Id.Value).ToList();
            if (linksOnPnr.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]La reserva no tiene pasajeros registrados.[/]");
                Pause();
                return;
            }

            var personRows = await context.Set<PersonEntity>()
                .AsNoTracking()
                .Where(p => linksOnPnr.Select(x => x.IdPerson).Contains(p.IdPerson))
                .ToListAsync(ct);

            var matches = linksOnPnr
                .Select(bc => (bc, personRows.FirstOrDefault(pe => pe.IdPerson == bc.IdPerson)))
                .Where(x => x.Item2 is not null && LastNameEquals(x.Item2!.LastName, surnameKey))
                .ToList();

            if (matches.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No hay pasajeros con ese apellido en esta reserva.[/]");
                Pause();
                return;
            }

            PersonEntity chosenPerson;
            if (matches.Count == 1)
                chosenPerson = matches[0].Item2!;
            else
            {
                var choices = matches
                    .Select((m, i) =>
                        $"{i + 1}. {m.Item2!.FirstName} {m.Item2.LastName} (doc. {m.Item2.DocumentNumber})")
                    .ToList();
                string pick = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Varios pasajeros coinciden con el apellido. Elegí uno:")
                        .PageSize(10)
                        .AddChoices(choices));
                var d = pick.IndexOf('.');
                var idx = int.Parse(pick.AsSpan(0, d), CultureInfo.InvariantCulture) - 1;
                idx = Math.Clamp(idx, 0, matches.Count - 1);
                chosenPerson = matches[idx].Item2!;
            }

            var legs = await BuildPaidLegsForPersonAsync(
                context, bookingRepo, allLinks, chosenPerson.IdPerson, booking0, paidId, allTickets, ct);

            if (legs.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No se encontraron tramos pagados con tiquete para este pasajero.[/]");
                Pause();
                return;
            }

            var ticketIdByBooking = allTickets
                .GroupBy(t => t.IdBooking)
                .ToDictionary(g => g.Key, g => g.OrderBy(t => t.Id.Value).First());

            RenderTripSummaryPanel(chosenPerson, legs, ticketIdByBooking, allCheckIns);

            (Booking bookingLeg, Flight flightLeg, string routeLabel, string legKindLabel) selected;
            if (legs.Count == 1)
            {
                selected = legs[0];
            }
            else
            {
                string choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title(
                            "[bold]Check-in por tramo:[/] elegí si ahora hacés el check-in de la ida o del regreso " +
                            "(cada uno es independiente).")
                        .PageSize(8)
                        .AddChoices(legs.Select((L, i) =>
                            $"{i + 1}. {L.legKindLabel} — {L.routeLabel} ({L.flightLeg.Date.Value:yyyy-MM-dd})")));
                var dot = choice.IndexOf('.');
                var opt = int.Parse(choice.AsSpan(0, dot), CultureInfo.InvariantCulture) - 1;
                selected = legs[Math.Clamp(opt, 0, legs.Count - 1)];
            }

            var ticketsOnLeg = allTickets
                .Where(t => t.IdBooking == selected.bookingLeg.Id.Value)
                .OrderBy(t => t.Id.Value)
                .ToList();
            if (ticketsOnLeg.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No hay tiquete emitido para este tramo. El check-in requiere tiquete.[/]");
                Pause();
                return;
            }

            var ticket = ticketsOnLeg[0];
            if (allCheckIns.Any(c => c.IdTicket == ticket.Id.Value))
            {
                AnsiConsole.MarkupLine("[red]El check-in ya fue realizado para este tramo (tiquete ya registrado).[/]");
                Pause();
                return;
            }

            if (selected.bookingLeg.IdStatus != paidId)
            {
                AnsiConsole.MarkupLine("[red]La reserva no está en estado Pagada. Completá el pago antes del check-in.[/]");
                Pause();
                return;
            }

            var legBc = allLinks.FirstOrDefault(l =>
                l.IdBooking == selected.bookingLeg.Id.Value && l.IdPerson == chosenPerson.IdPerson);
            if (legBc is null)
            {
                AnsiConsole.MarkupLine("[red]No se encontró al pasajero en la reserva de este tramo.[/]");
                Pause();
                return;
            }

            await BookingPaidBundleBaggageProvisioner.TryProvisionIfNoBaggageAsync(
                context,
                ticket.Id.Value,
                selected.bookingLeg.Observations.Value,
                selected.bookingLeg.SeatCount.Value,
                ct);
            await context.SaveChangesAsync(ct);

            var otherLegCheckedIn = legs.Count > 1 && legs.Any(l =>
                l.bookingLeg.Id.Value != selected.bookingLeg.Id.Value
                && ticketIdByBooking.TryGetValue(l.bookingLeg.Id.Value, out var tAgg)
                && allCheckIns.Any(c => c.IdTicket == tAgg.Id.Value));

            var seatRepo = new SeatRepository(context);
            var allSeats = await new GetAllSeatsUseCase(seatRepo).ExecuteAsync(ct);
            var seatMap = allSeats.ToDictionary(s => s.Id.Value, s => s);

            var currentSeatId = legBc.IdSeat;
            var currentSeatLabel = currentSeatId > 0 && seatMap.TryGetValue(currentSeatId, out var cs)
                ? cs.Number.Value
                : "(sin asiento)";

            AnsiConsole.Write(new Panel(
                    $"[bold]Pasajero:[/] {Markup.Escape(chosenPerson.FirstName)} {Markup.Escape(chosenPerson.LastName)}\n" +
                    $"[bold]Vuelo:[/] {Markup.Escape(selected.flightLeg.Number.Value)}\n" +
                    $"[bold]Ruta:[/] {Markup.Escape(selected.routeLabel)}\n" +
                    $"[bold]Salida:[/] {selected.flightLeg.Date.Value:yyyy-MM-dd} {selected.flightLeg.DepartureTime.Value:hh\\:mm}\n" +
                    $"[bold]Pago:[/] Pagada\n" +
                    $"[bold]Check-in este tramo:[/] Pendiente\n" +
                    (legs.Count > 1
                        ? $"[bold]Otro tramo:[/] {(otherLegCheckedIn ? "Ya con check-in" : "Pendiente de check-in")}\n"
                        : "") +
                    $"[bold]Asiento asignado:[/] {Markup.Escape(currentSeatLabel)}\n" +
                    $"[bold]Tiquete:[/] {Markup.Escape(ticket.Code.Value)}")
                .Header("[cyan]Datos del trayecto[/]")
                .Border(BoxBorder.Rounded));

            var baggages = (await new GetAllBaggagesUseCase(new BaggageRepository(context)).ExecuteAsync(ct))
                .Where(b => b.IdTicket == ticket.Id.Value)
                .ToList();
            var baggageTypes = await new GetAllBaggageTypesUseCase(new BaggageTypeRepository(context)).ExecuteAsync(ct);
            var typeNames = baggageTypes.ToDictionary(t => t.Id.Value, t => t.Name.Value);

            void WriteBaggage()
            {
                if (baggages.Count == 0)
                    AnsiConsole.MarkupLine("[grey]Equipaje registrado: ninguno.[/]");
                else
                {
                    AnsiConsole.MarkupLine("[bold]Equipaje registrado:[/]");
                    foreach (var b in baggages)
                    {
                        var tn = typeNames.TryGetValue(b.IdBaggageType, out var n) ? n : b.IdBaggageType.ToString();
                        AnsiConsole.MarkupLine($"  • {Markup.Escape(tn)} — {b.Weight.Value:F1} kg");
                    }
                }
            }

            WriteBaggage();

            while (AnsiConsole.Confirm("\n¿Deseás agregar otra pieza de equipaje?", false))
            {
                var weight = AnsiConsole.Prompt(
                    new TextPrompt<decimal>($"Peso en kg (máx. {BaggageWeight.MaximumKilograms:0} por pieza):")
                        .Validate(v =>
                        {
                            if (v <= 0)
                                return ValidationResult.Error("[red]Debe ser mayor a 0.[/]");
                            if (v > BaggageWeight.MaximumKilograms)
                                return ValidationResult.Error($"[red]Máximo {BaggageWeight.MaximumKilograms:0} kg.[/]");
                            return ValidationResult.Success();
                        }));
                var typeItems = await new GetAllBaggageTypesUseCase(new BaggageTypeRepository(context)).ExecuteAsync(ct);
                if (typeItems.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]No hay tipos de equipaje en el sistema.[/]");
                    break;
                }

                string typePick = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Tipo de equipaje:")
                        .AddChoices(typeItems.Select(t => $"{t.Id.Value}. {t.Name.Value}")));
                var dType = typePick.IndexOf('.');
                var idType = int.Parse(typePick.AsSpan(0, dType), CultureInfo.InvariantCulture);
                await new CreateBaggageUseCase(new BaggageRepository(context))
                    .ExecuteAsync(weight, ticket.Id.Value, idType, ct);
                await context.SaveChangesAsync(ct);
                baggages = (await new GetAllBaggagesUseCase(new BaggageRepository(context)).ExecuteAsync(ct))
                    .Where(b => b.IdTicket == ticket.Id.Value)
                    .ToList();
                AnsiConsole.MarkupLine("[green]Equipaje agregado.[/]");
                WriteBaggage();
            }

            var finalSeatId = currentSeatId;
            if (currentSeatId > 0 && AnsiConsole.Confirm("¿Deseás cambiar de asiento?", false))
            {
                var picked = await PromptSeatChangeAsync(
                    context, selected.flightLeg.Id.Value, currentSeatId, seatMap, ct);
                if (picked is int newSeat && newSeat != currentSeatId)
                {
                    await ApplySeatChangeAsync(context, legBc, selected.bookingLeg.IdFlight, currentSeatId, newSeat, ct);
                    finalSeatId = newSeat;
                    currentSeatLabel = seatMap[newSeat].Number.Value;
                }
            }
            else if (currentSeatId <= 0)
            {
                var picked = await PromptSeatChangeAsync(
                    context, selected.flightLeg.Id.Value, 0, seatMap, ct);
                if (picked is not int forced || forced <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Debés elegir un asiento para continuar.[/]");
                    Pause();
                    return;
                }

                await ApplySeatChangeAsync(context, legBc, selected.bookingLeg.IdFlight, 0, forced, ct);
                finalSeatId = forced;
                currentSeatLabel = seatMap[forced].Number.Value;
            }

            var baggageSummary = baggages.Count == 0
                ? "Ninguno"
                : string.Join(", ", baggages.Select(b =>
                {
                    var tn = typeNames.TryGetValue(b.IdBaggageType, out var n) ? n : "?";
                    return $"{tn} {b.Weight.Value:F1}kg";
                }));

            AnsiConsole.Write(new Panel(
                    $"[bold]Tramo:[/] {Markup.Escape(selected.legKindLabel)} — {Markup.Escape(selected.routeLabel)}\n" +
                    $"[bold]Asiento final:[/] {Markup.Escape(currentSeatLabel)}\n" +
                    $"[bold]Equipaje total:[/] {Markup.Escape(baggageSummary)}")
                .Header("[yellow]Resumen[/]")
                .Border(BoxBorder.Double));

            if (!AnsiConsole.Confirm("¿Confirmar check-in?", true))
            {
                AnsiConsole.MarkupLine("[grey]Check-in cancelado.[/]");
                Pause();
                return;
            }

            const int webChannelId = 1;
            await new CreateCheckInUseCase(new CheckInRepository(context))
                .ExecuteAsync(DateTime.UtcNow, ticket.Id.Value, webChannelId, finalSeatId, AppState.IdUser, checkInCompletedId, ct);
            await context.SaveChangesAsync(ct);

            AnsiConsole.MarkupLine("\n[green]Check-in registrado correctamente.[/]\n");
            RenderBoardingPass(
                chosenPerson,
                selected.flightLeg,
                selected.routeLabel,
                currentSeatLabel,
                selected.legKindLabel);

            Pause();
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
            Pause();
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

    private static void RenderBoardingPass(
        PersonEntity person,
        Flight flight,
        string routeLabel,
        string seatNumber,
        string legKind)
    {
        var gate = $"{(char)('A' + Random.Shared.Next(0, 3))}{Random.Shared.Next(1, 30):D2}";
        var group = Random.Shared.Next(1, 6);
        var panel = new Panel(
                $"[bold]Pasajero:[/] {Markup.Escape(person.FirstName)} {Markup.Escape(person.LastName)}\n" +
                $"[bold]Vuelo:[/] {Markup.Escape(flight.Number.Value)}\n" +
                $"[bold]Ruta:[/] {Markup.Escape(routeLabel)}\n" +
                $"[bold]Fecha y hora:[/] {flight.Date.Value:yyyy-MM-dd} {flight.DepartureTime.Value:hh\\:mm}\n" +
                $"[bold]Asiento:[/] {Markup.Escape(seatNumber)}\n" +
                $"[bold]Puerta:[/] {gate}\n" +
                $"[bold]Grupo de abordaje:[/] {group}\n" +
                $"[bold]Tipo de trayecto:[/] {Markup.Escape(legKind)}")
            .Header("[green]PASE DE ABORDAR[/]")
            .Border(BoxBorder.Heavy);
        AnsiConsole.Write(panel);
    }

    private static async Task<int?> PromptSeatChangeAsync(
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

        var choices = selectableIds
            .Where(id => seatMap.ContainsKey(id))
            .Select(id => $"{id}. {seatMap[id].Number.Value}")
            .ToList();
        if (choices.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No hay asientos disponibles para este vuelo.[/]");
            return null;
        }

        string sel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Elegí asiento:")
                .PageSize(15)
                .AddChoices(choices));
        var dSel = sel.IndexOf('.');
        var idSeat = int.Parse(sel.AsSpan(0, dSel), CultureInfo.InvariantCulture);
        return idSeat;
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
        await context.SaveChangesAsync(ct);
    }

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

    private static bool LastNameEquals(string dbLastName, string input) =>
        string.Equals(dbLastName.Trim(), input.Trim(), StringComparison.OrdinalIgnoreCase);

    private static async Task<bool> UserMayAccessBookingAsync(AppDbContext context, int idBooking, CancellationToken ct)
    {
        if (AppState.IdUserRole == 1)
            return true;

        var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
        foreach (var l in links)
        {
            if (l.IdBooking == idBooking && (l.IdUser == AppState.IdUser || (AppState.IdPerson is int pid && l.IdPerson == pid)))
                return true;
        }

        var all = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        var b = all.FirstOrDefault(x => x.Id.Value == idBooking);
        if (b is null)
            return false;
        return ClientBookingCodeOwnership.CodeLooksLikeClientGeneratedBooking(b.Code.Value, AppState.IdUser);
    }

    private static void Pause()
    {
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey(true);
    }
}
