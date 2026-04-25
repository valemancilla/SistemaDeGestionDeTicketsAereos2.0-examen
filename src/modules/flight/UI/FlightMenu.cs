using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Repositories;
using AirAggregate = SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.aggregate.Aircraft;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Repositories;
using System.Globalization;
using System.Linq;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Domain;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.UI;

public sealed class FlightMenu
{
    private const string BaggageCarryOnName = "Equipaje de mano";
    private const string BaggageCheckedName = "Equipaje de bodega";

    /// <summary>Mismos datos que el cliente ingresa en «Buscar vuelos» antes de consultar la base.</summary>
    private sealed record ClientTripSearchCriteria(
        bool RoundTrip,
        int IdOriginAirport,
        int IdDestinationAirport,
        DateOnly DateOutbound,
        DateOnly? DateReturn,
        int SeatCount);

    /// <summary>Datos de publicación (admin) para crear vuelos usando el mismo concepto de búsqueda, pero el cupo lo define el admin.</summary>
    private sealed record AdminPublishCriteria(
        bool RoundTrip,
        int IdOriginAirport,
        int IdDestinationAirport,
        DateOnly DateOutbound,
        DateOnly? DateReturn,
        int SeatsToSell);

    /// <param name="roundTripKnown">Si tiene valor, no se pregunta tipo de viaje (uso admin: solo ida o ida y vuelta ya elegidos en menú). Si es null, se pregunta (cliente en Buscar vuelos).</param>
    /// <returns><see langword="null"/> si origen y destino son el mismo (flujo cancelado).</returns>
    private static async Task<ClientTripSearchCriteria?> PromptClientTripSearchCriteriaAsync(CancellationToken ct, bool? roundTripKnown = null)
    {
        bool roundTrip;
        if (roundTripKnown is bool preset)
        {
            roundTrip = preset;
            AnsiConsole.MarkupLine(
                preset
                    ? "\n[grey]Publicando: [bold]Ida y vuelta[/] — ida: ciudad A→ciudad B; vuelta (regreso): B→A (ej. Bucaramanga→Bogotá y después Bogotá→Bucaramanga).[/]"
                    : "\n[grey]Publicando: [bold]Solo ida[/] — un solo tramo en sentido origen→destino (ej. Bucaramanga→Bogotá).[/]");
        }
        else
        {
            var tripKind = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Tipo de viaje:")
                    .PageSize(3)
                    .AddChoices("Ida y vuelta", "Solo ida"));
            roundTrip = tripKind == "Ida y vuelta";
            if (roundTrip)
                AnsiConsole.MarkupLine(
                    "[grey]Ida y vuelta = salís de una ciudad a otra en la ida, y el regreso es al revés (ej. ida Bucaramanga→Bogotá, vuelta Bogotá→Bucaramanga).[/]");
        }

        var idOriginAirport = await SelectAirportAsync(
            "IDA — Aeropuerto de ORIGEN (desde dónde salís en la ida, ej. Bucaramanga):", ct);
        var idDestinationAirport = await SelectAirportAsync(
            "IDA — Aeropuerto de DESTINO (a dónde llegás en la ida, ej. Bogotá):", ct);
        if (idOriginAirport == idDestinationAirport)
        {
            AnsiConsole.MarkupLine("\n[red]El origen y el destino no pueden ser el mismo.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return null;
        }

        if (roundTrip)
            AnsiConsole.MarkupLine(
                "[grey]El tramo de [bold]vuelta[/] (regreso) va del [bold]destino de la ida[/] al [bold]origen de la ida[/]: misma pareja de aeropuertos en sentido inverso.[/]");

        var dateOutStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Fecha de IDA (yyyy-MM-dd):")
                .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido[/]")));
        var dateOutbound = DateOnly.ParseExact(dateOutStr, "yyyy-MM-dd");

        DateOnly? dateReturn = null;
        if (roundTrip)
        {
            var dateRetStr = AnsiConsole.Prompt(
                new TextPrompt<string>("Fecha de VUELTA / REGRESO (vuelo destino→origen, yyyy-MM-dd):")
                    .Validate(s =>
                    {
                        if (!DateOnly.TryParseExact(s, "yyyy-MM-dd", out var d))
                            return ValidationResult.Error("[red]Formato inválido[/]");
                        if (d < dateOutbound)
                            return ValidationResult.Error("[red]La vuelta no puede ser anterior a la ida.[/]");
                        return ValidationResult.Success();
                    }));
            dateReturn = DateOnly.ParseExact(dateRetStr, "yyyy-MM-dd");
        }

        var seatCount = AnsiConsole.Prompt(
            new TextPrompt<int>("¿Cuántas personas van a viajar? (asientos a reservar en esta búsqueda)")
                .Validate(v => v > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Debe ser al menos 1[/]")));

        return new ClientTripSearchCriteria(
            roundTrip,
            idOriginAirport,
            idDestinationAirport,
            dateOutbound,
            dateReturn,
            seatCount);
    }

    private static async Task<AdminPublishCriteria?> PromptAdminPublishCriteriaAsync(CancellationToken ct, bool? roundTripKnown = null)
    {
        bool roundTrip;
        if (roundTripKnown is bool preset)
        {
            roundTrip = preset;
            AnsiConsole.MarkupLine(
                preset
                    ? "\n[grey]Publicando: [bold]Ida y vuelta[/] — se crearán 2 vuelos: ida (A→B) y regreso (B→A).[/]"
                    : "\n[grey]Publicando: [bold]Solo ida[/] — se creará 1 vuelo: origen→destino.[/]");
        }
        else
        {
            var tripKind = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Tipo de publicación:")
                    .PageSize(3)
                    .AddChoices("Ida y vuelta", "Solo ida"));
            roundTrip = tripKind == "Ida y vuelta";
        }

        var idOriginAirport = await SelectAirportAsync(
            "IDA — Aeropuerto de ORIGEN (desde dónde sale el vuelo de ida):", ct);
        var idDestinationAirport = await SelectAirportAsync(
            "IDA — Aeropuerto de DESTINO (a dónde llega el vuelo de ida):", ct);
        if (idOriginAirport == idDestinationAirport)
        {
            AnsiConsole.MarkupLine("\n[red]El origen y el destino no pueden ser el mismo.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return null;
        }

        var dateOutStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Fecha de IDA (yyyy-MM-dd):")
                .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido[/]")));
        var dateOutbound = DateOnly.ParseExact(dateOutStr, "yyyy-MM-dd");

        DateOnly? dateReturn = null;
        if (roundTrip)
        {
            var dateRetStr = AnsiConsole.Prompt(
                new TextPrompt<string>("Fecha de VUELTA / REGRESO (destino→origen, yyyy-MM-dd):")
                    .Validate(s =>
                    {
                        if (!DateOnly.TryParseExact(s, "yyyy-MM-dd", out var d))
                            return ValidationResult.Error("[red]Formato inválido[/]");
                        if (d < dateOutbound)
                            return ValidationResult.Error("[red]La vuelta no puede ser anterior a la ida.[/]");
                        return ValidationResult.Success();
                    }));
            dateReturn = DateOnly.ParseExact(dateRetStr, "yyyy-MM-dd");
        }

        var seatsToSell = AnsiConsole.Prompt(
            new TextPrompt<int>("Asientos disponibles para vender (reservables desde el inicio):")
                .Validate(v => v > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Debe ser al menos 1[/]")));

        return new AdminPublishCriteria(
            roundTrip,
            idOriginAirport,
            idDestinationAirport,
            dateOutbound,
            dateReturn,
            seatsToSell);
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE VUELOS[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(8)
                    .AddChoices("1. Crear vuelo (flujo guiado)", "2. Listar vuelos",
                                "3. Actualizar vuelo", "4. Registrar cambio de estado",
                                "5. Eliminar vuelo", "0. Volver"));

            switch (option)
            {
                case "1. Crear vuelo (flujo guiado)":
                    {
                        var mode = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Publicación (ADMIN): todos los vuelos se crean por [bold]rutas existentes[/].")
                                .PageSize(5)
                                .AddChoices(
                                    "Solo IDA (1 vuelo)",
                                    "IDA y VUELTA (2 vuelos: ida + regreso)",
                                    "0. Volver"));
                        if (mode.StartsWith("0.", StringComparison.Ordinal))
                            break;
                        if (mode.StartsWith("Solo IDA", StringComparison.Ordinal))
                            await CreateGuidedAsync(ct);
                        else
                            await CreateRoundTripFromExistingRoutesAsync(ct);
                        break;
                    }
                case "2. Listar vuelos": await ListAsync(ct); break;
                case "3. Actualizar vuelo": await UpdateAsync(ct); break;
                case "4. Registrar cambio de estado": await AddStatusHistoryAsync(ct); break;
                case "5. Eliminar vuelo": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    public async Task ShowAvailableFlightsAsync(CancellationToken ct = default)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]BUSCAR VUELOS[/]").Centered());
        AnsiConsole.MarkupLine(
            "[grey]Tipo de viaje, origen y destino de la [bold]ida[/], fechas y pasajeros. " +
            "Si es ida y vuelta, el regreso es del destino al origen (ej. ida Bucaramanga→Bogotá, vuelta Bogotá→Bucaramanga).[/]");
        AnsiConsole.MarkupLine(
            "[grey]Cupo: se muestra como [bold]venta/capacidad[/] (ej. 3/9 = quedan 3 asientos a la venta de 9 de capacidad del vuelo; el primero baja con cada reserva).[/]\n");

        var criteria = await PromptClientTripSearchCriteriaAsync(ct, roundTripKnown: null);
        if (criteria is null)
            return;

        var roundTrip = criteria.RoundTrip;
        var idOriginAirport = criteria.IdOriginAirport;
        var idDestinationAirport = criteria.IdDestinationAirport;
        var dateOutbound = criteria.DateOutbound;
        DateOnly? dateReturn = criteria.DateReturn;
        var seatCount = criteria.SeatCount;

        using var context = DbContextFactory.Create();
        var routes = await new GetAllRoutesUseCase(new RouteRepository(context)).ExecuteAsync(ct);
        var flights = await new GetAllFlightsUseCase(new FlightRepository(context)).ExecuteAsync(ct);
        var airports = await new GetAllAirportsUseCase(new AirportRepository(context)).ExecuteAsync(ct);
        var aircrafts = await new GetAllAircraftsUseCase(new AircraftRepository(context)).ExecuteAsync(ct);
        var aerolines = await new GetAllAerolinesUseCase(new AerolineRepository(context)).ExecuteAsync(ct);
        var fares = await new GetAllFaresUseCase(new FareRepository(context)).ExecuteAsync(ct);

        static List<Flight> FlightsOnRouteDate(
            IReadOnlyList<Flight> allFlights,
            IReadOnlyList<Route> allRoutes,
            HashSet<int> originAirportIds,
            HashSet<int> destAirportIds,
            DateOnly date)
        {
            // No exigir ruta «activa»: si el vuelo existe pero la ruta quedó inactiva, igual debe mostrarse (con aviso al reservar).
            var routeIds = allRoutes
                .Where(r => originAirportIds.Contains(r.OriginAirport) && destAirportIds.Contains(r.DestinationAirport))
                .Select(r => r.Id.Value)
                .ToHashSet();

            return allFlights
                .Where(f => routeIds.Contains(f.IdRoute) && f.Date.Value == date)
                .OrderBy(f => f.DepartureTime.Value)
                .ToList();
        }

        var originIds = AirportSearchEquivalence.ExpandAirportIdsForMatching(airports, idOriginAirport);
        var destIds = AirportSearchEquivalence.ExpandAirportIdsForMatching(airports, idDestinationAirport);
        if (originIds.Count > 1 || destIds.Count > 1)
            AnsiConsole.MarkupLine("[grey]Búsqueda ampliada: se tratan como el mismo punto los aeropuertos equivalentes del catálogo (p. ej. MDE y MED).[/]");

        var outboundOnDate = FlightsOnRouteDate(flights, routes, originIds, destIds, dateOutbound);
        var outboundRaw = outboundOnDate.Where(f => f.AvailableSeats.Value >= seatCount).ToList();

        List<Flight> inboundOnDate = new();
        List<Flight> inboundRaw = new();
        if (roundTrip && dateReturn is DateOnly drLeg)
        {
            inboundOnDate = FlightsOnRouteDate(flights, routes, destIds, originIds, drLeg);
            inboundRaw = inboundOnDate.Where(f => f.AvailableSeats.Value >= seatCount).ToList();
        }

        var routeById = routes.ToDictionary(r => r.Id.Value);
        var airportById = airports.ToDictionary(a => a.Id.Value);
        var aircraftById = aircrafts.ToDictionary(a => a.Id.Value);
        var airlineNameById = aerolines.ToDictionary(a => a.Id.Value, a => a.Name.Value);

        var originIata = airportById.TryGetValue(idOriginAirport, out var oa) ? oa.IATACode.Value : "?";
        var destIata = airportById.TryGetValue(idDestinationAirport, out var da) ? da.IATACode.Value : "?";

        var hasReturnRoute = routes.Any(r => r.Active && destIds.Contains(r.OriginAirport) && originIds.Contains(r.DestinationAirport));
        if (roundTrip && !hasReturnRoute)
        {
            AnsiConsole.MarkupLine(
                $"\n[yellow]No hay ruta activa de vuelta ({Markup.Escape(destIata)} → {Markup.Escape(originIata)}). " +
                "Solo podrás ver y reservar la ida hasta que exista esa ruta (Vuelos → Crear vuelo → [bold]IDA y VUELTA[/]).[/]");
            WriteReturnRouteAlternativesHint(routes, airportById, idOriginAirport, idDestinationAirport, originIata, destIata);
        }
        List<Flight>? inbound = null;
        List<FlightSearchOffer>? inboundOfferList = null;

        // Listar todos los vuelos del día en ese trayecto; el cupo para N pasajeros se valida al reservar.
        var fareIdsForClassPricing = FareIdsFromFlightsForPricing(outboundOnDate)
            .Concat(FareIdsFromFlightsForPricing(inboundOnDate))
            .Distinct()
            .ToList();
        var seatClassPricesByFare = await FareSeatClassPricingHelper.LoadSeatClassPricesByFareIdAsync(context, fareIdsForClassPricing, ct);

        var outboundOffers = BuildFlightOffers(
            outboundOnDate.ToList(), dateOutbound, routeById, airportById, aircraftById, airlineNameById, fares, seatClassPricesByFare);
        var outbound = outboundOffers.Select(o => o.Flight).ToList();

        if (outboundOnDate.Count == 0)
            WriteSameDayOtherDestinationsFromOrigin(flights, routeById, airportById, dateOutbound, idOriginAirport, idDestinationAirport);

        WriteFlightOffersSection(
            $"VUELOS DE IDA ({originIata} → {destIata})",
            dateOutbound,
            outboundOnDate,
            outboundRaw.Count,
            outboundOffers,
            seatCount);
        if (roundTrip)
        {
            AnsiConsole.WriteLine();
            if (inboundOnDate.Count > 0)
            {
                var inboundOffers = BuildFlightOffers(
                    inboundOnDate.ToList(), dateReturn!.Value, routeById, airportById, aircraftById, airlineNameById, fares, seatClassPricesByFare);
                inbound = inboundOffers.Select(o => o.Flight).ToList();
                inboundOfferList = inboundOffers;
                WriteFlightOffersSection(
                    $"VUELOS DE VUELTA ({destIata} → {originIata})",
                    dateReturn!.Value,
                    inboundOnDate,
                    inboundRaw.Count,
                    inboundOffers,
                    seatCount);
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]No hay vuelos de vuelta publicados[/] para [bold]{dateReturn!.Value:yyyy-MM-dd}[/] ({Markup.Escape(destIata)} → {Markup.Escape(originIata)}).");
                AnsiConsole.MarkupLine("[grey]Un administrador puede publicarlos con Vuelos → Crear vuelo → [bold]IDA y VUELTA[/].[/]");
                WriteSameDayFlightsToOriginFromOtherAirports(
                    flights, routeById, airportById, dateReturn!.Value, idOriginAirport, idDestinationAirport);
            }
        }

        if (outboundOnDate.Count == 0 && (!roundTrip || inboundOnDate.Count == 0))
        {
            AnsiConsole.MarkupLine("\n[yellow]No hay opciones para reservar con los datos ingresados.[/]");
            AnsiConsole.MarkupLine("[grey]El detalle por tramo está en los bloques de ida/vuelta más arriba.[/]");

            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return;
        }

        var canBookIda = outbound.Count > 0;
        var canBookVuelta = roundTrip && inbound is { Count: > 0 };
        var canBookRoundTripPair = canBookIda && canBookVuelta;

        var actionChoices = new List<string>();
        if (canBookRoundTripPair)
        {
            actionChoices.Add("1. Reservar ida y vuelta (ambos tramos; no se confirma solo ida ni solo vuelta)");
        }
        else
        {
            if (canBookIda)
                actionChoices.Add("1. Reservar vuelo de IDA");
            if (canBookVuelta)
                actionChoices.Add(canBookIda ? "2. Reservar vuelo de VUELTA" : "1. Reservar vuelo de VUELTA");
        }
        actionChoices.Add("0. Volver sin reservar");

        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("\n¿Qué deseas hacer?").PageSize(6).AddChoices(actionChoices));

        if (action == "0. Volver sin reservar")
        {
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return;
        }

        if (canBookRoundTripPair && action.StartsWith("1.", StringComparison.Ordinal))
        {
            var idaPick = await TryPickRoundTripLegAsync(outbound, "IDA", seatCount, ct, outboundOffers, routeById);
            if (idaPick is null)
                return;

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[cyan]Siguiente: vuelo de VUELTA (obligatorio para cerrar el paquete ida y vuelta).[/]");
            var vuePick = await TryPickRoundTripLegAsync(inbound!, "VUELTA", seatCount, ct, inboundOfferList, routeById);
            if (vuePick is null)
                return;

            AnsiConsole.MarkupLine(
                "[grey]Tras confirmar, vas a elegir asientos en [bold]ida[/] y luego en [bold]vuelta[/]. " +
                "Después completarás [bold]una sola vez[/] los datos de cada pasajero (ida+vuelta), elegirás titular y contacto, y al final verás el resumen con códigos, tiquetes, asientos y totales.[/]");
            if (!AnsiConsole.Confirm("\n¿Confirmar y continuar a elegir asientos (ida y luego vuelta)?", true))
            {
                AnsiConsole.MarkupLine("[grey]Reserva cancelada. Presiona cualquier tecla para continuar...[/]");
                Console.ReadKey();
                return;
            }

            var (idaOk, idaBookingId) = await new BookingMenu().CreateFromSelectedFlightAsync(
                idaPick.FlightId,
                idaPick.FlightDateTime,
                ct,
                seatsFromSearch: seatCount,
                bundledObservationPrefix: idaPick.BundledObservation,
                skipIntroConfirm: true,
                seatSelectionFlightLegLabel: "IDA",
                deferPassengerDetailsCompletion: true);
            if (!idaOk)
                return;

            var (vueOk, vueBookingId) = await new BookingMenu().CreateFromSelectedFlightAsync(
                vuePick.FlightId,
                vuePick.FlightDateTime,
                ct,
                seatsFromSearch: seatCount,
                bundledObservationPrefix: vuePick.BundledObservation,
                skipIntroConfirm: true,
                seatSelectionFlightLegLabel: "VUELTA",
                deferPassengerDetailsCompletion: true);
            if (!vueOk && idaBookingId > 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]No se completó la vuelta: se intenta anular la reserva de ida para no quedar con un solo tramo.[/]");
                var undone = await BookingMenu.TryUndoOutboundBookingAfterFailedReturnLegAsync(idaBookingId, ct);
                if (undone)
                    AnsiConsole.MarkupLine("[green]Listo: la ida quedó anulada y el cupo del vuelo de ida se restauró.[/]");
            }
            else if (idaOk && vueOk && idaBookingId > 0 && vueBookingId > 0)
            {
                var holderFlowOk = await BookingMenu.CompletePassengerDetailsForRoundTripBookingsAsync(idaBookingId, vueBookingId, ct);
                if (AppState.IdUserRole != 1 && holderFlowOk)
                    await BookingMenu.RunClientCheckoutForBookingsAsync(new[] { idaBookingId, vueBookingId }, ct);
                await WriteRoundTripTravelSummaryAfterBookingsAsync(
                    idaPick, vuePick, seatCount, airportById, idaBookingId, vueBookingId, ct);
            }

            return;
        }

        List<Flight> legList;
        string legTitle;
        if (action.Contains("VUELTA", StringComparison.Ordinal))
        {
            legList = inbound!;
            legTitle = "VUELTA";
        }
        else
        {
            legList = outbound;
            legTitle = "IDA";
        }

        await TrySelectAndBookFlightLegAsync(
            legList,
            legTitle,
            seatCount,
            ct,
            legTitle == "IDA" ? outboundOffers : inboundOfferList);
    }

    /// <param name="pricingOffers">Ofertas con precio mínimo por vuelo; sirve para la tarifa Basic/Classic/Flex en ida y vuelta (cliente).</param>
    /// <returns>Éxito y ID de reserva (0 si no se guardó).</returns>
    private static async Task<(bool success, int bookingId)> TrySelectAndBookFlightLegAsync(
        List<Flight> legList,
        string legTitle,
        int seatCount,
        CancellationToken ct,
        IReadOnlyList<FlightSearchOffer>? pricingOffers = null)
    {
        var choices = legList
            .OrderBy(f => f.Id.Value)
            .Select(f =>
                $"{f.Id.Value}. {f.Number.Value} — {f.Date.Value:yyyy-MM-dd} {f.DepartureTime.Value:HH\\:mm} (cupo {FormatFlightCupoVentaVsCapacidad(f)} venta/cap.)")
            .ToList();
        choices.Add("0. Cancelar");

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"\nSelecciona tu vuelo de {legTitle}:")
                .PageSize(12)
                .AddChoices(choices));

        if (selected == "0. Cancelar")
        {
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return (false, 0);
        }

        var id = int.Parse(selected.Split('.')[0], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
        var flight = legList.First(f => f.Id.Value == id);
        var flightDateTime = flight.Date.Value.ToDateTime(flight.DepartureTime.Value);

        if (flight.AvailableSeats.Value < seatCount)
        {
            AnsiConsole.MarkupLine(
                $"\n[red]Este vuelo de {legTitle} no tiene cupo para las {seatCount} persona(s) de la búsqueda. " +
                $"Cupo a la venta ahora: [bold]{flight.AvailableSeats.Value}[/] de [bold]{flight.TotalCapacity.Value}[/] de capacidad del vuelo " +
                $"(si la capacidad es mayor que lo disponible, el resto ya está reservado o el cupo inicial se fijó menor al crear el vuelo).[/]");
            AnsiConsole.MarkupLine("[grey]Elegí otro vuelo o volvé a buscar con otra cantidad de pasajeros.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return (false, 0);
        }

        string? bundledObservation = null;
        if ((legTitle == "IDA" || legTitle == "VUELTA") && AppState.IdUserRole != 1)
        {
            const decimal bundleReferenceWithoutPublishedFare = 250_000m;
            var offerForSelected = pricingOffers?.FirstOrDefault(o => o.Flight.Id.Value == id);
            var hasPublishedFare = offerForSelected?.MinPrice is decimal;
            var baseForBundle = hasPublishedFare ? offerForSelected!.MinPrice!.Value : bundleReferenceWithoutPublishedFare;
            if (!hasPublishedFare)
            {
                AnsiConsole.MarkupLine(
                    "[grey]No hay tarifa publicada para este vuelo en esta fecha; igual podés elegir tipo de equipaje/tarifa (Basic/Classic/Flex) con montos de referencia.[/]");
            }

            AnsiConsole.WriteLine();
                    bundledObservation = await PromptFareBundleSelectionForLegClientAsync(baseForBundle, legTitle);
            if (bundledObservation is null)
            {
                AnsiConsole.MarkupLine("[grey]Reserva cancelada (no elegiste tarifa o elegiste volver).[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
                return (false, 0);
            }

            if (!hasPublishedFare)
                bundledObservation += " [ref sin tarifa publicada en sistema]";
        }

        return await new BookingMenu().CreateFromSelectedFlightAsync(
            id,
            flightDateTime,
            ct,
            seatsFromSearch: seatCount,
            bundledObservationPrefix: bundledObservation,
            seatSelectionFlightLegLabel: legTitle);
    }

    private static Panel BuildFareBundleTierPanel(string title, string hex, string bodyMarkup, decimal price, bool showBestRibbon)
    {
        var ribbon = showBestRibbon ? "[bold white on #6d28d9]★ MEJOR OPCIÓN[/]\n\n" : "";
        var foot = $"\n\n[bold white on {hex}] {FormatPriceCopColombia(price)} [/]\n[grey]Precio por pasajero[/]";
        return new Panel(new Markup(ribbon + bodyMarkup + foot))
            .Header($"[bold {hex}]{Markup.Escape(title)}[/]", Justify.Center)
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(Color.FromHex(hex)));
    }

    private static void WriteFareBundleComparisonCards(
        ClientFareBundleDisplayData policy,
        decimal refCarryOn,
        decimal refChecked,
        decimal pBasic,
        decimal pClassic,
        decimal pFlex)
    {
        var basicWithBags = decimal.Round(pBasic + refCarryOn + refChecked, 0, MidpointRounding.AwayFromZero);
        var basicBody = ClientFareBundleDisplayDefaults.ApplyPricePlaceholders(
            policy.BasicBodyMarkup, refCarryOn, refChecked, policy.SeatSelectionFromCop);
        var classicBody = ClientFareBundleDisplayDefaults.ApplyPricePlaceholders(
            policy.ClassicBodyMarkup, refCarryOn, refChecked, policy.SeatSelectionFromCop);
        var flexBody = ClientFareBundleDisplayDefaults.ApplyPricePlaceholders(
            policy.FlexBodyMarkup, refCarryOn, refChecked, policy.SeatSelectionFromCop);

        var p1 = BuildFareBundleTierPanel("Basic", "#db2777", basicBody, basicWithBags, showBestRibbon: false);
        var p2 = BuildFareBundleTierPanel("Classic", "#6d28d9", classicBody, pClassic, showBestRibbon: true);
        var p3 = BuildFareBundleTierPanel("Flex", "#ea580c", flexBody, pFlex, showBestRibbon: false);

        var row = new Table { ShowHeaders = false, Expand = true };
        row.Border(TableBorder.None);
        row.AddColumn(new TableColumn(string.Empty));
        row.AddColumn(new TableColumn(string.Empty));
        row.AddColumn(new TableColumn(string.Empty));
        row.AddRow(p1, p2, p3);
        AnsiConsole.Write(row);
    }

    /// <summary>Selector Basic/Classic/Flex por tramo (ida o vuelta). Devuelve observación, precio de referencia por pasajero y nombre de tarifa; null si cancela.</summary>
    private static async Task<(string Observation, decimal PricePerPerson, string TierName)?> PromptFareBundleSelectionForLegClientWithPriceAsync(decimal basePrice, string legTitle)
    {
        using var context = DbContextFactory.Create();
        var policy = await new GetClientFareBundleDisplayUseCase(new ClientFareBundleDisplayRepository(context)).ExecuteAsync(ct: default);
        var baggageTypes = await new GetAllBaggageTypesUseCase(new BaggageTypeRepository(context)).ExecuteAsync(ct: default);
        var carry = baggageTypes.FirstOrDefault(x => string.Equals(x.Name.Value, BaggageCarryOnName, StringComparison.OrdinalIgnoreCase));
        var checkedB = baggageTypes.FirstOrDefault(x => string.Equals(x.Name.Value, BaggageCheckedName, StringComparison.OrdinalIgnoreCase));
        var refCarryOn = carry?.BasePriceCop ?? policy.RefCarryOnCop;
        var refChecked = checkedB?.BasePriceCop ?? policy.RefCheckedCop;

        var pBasic = decimal.Round(basePrice, 0);
        var pClassic = decimal.Round(basePrice * policy.ClassicMultiplier, 0);
        var pFlex = decimal.Round(basePrice * policy.FlexMultiplier, 0);
        var basicBaggageRef = refCarryOn + refChecked;
        var pBasicTotal = decimal.Round(pBasic + basicBaggageRef, 0, MidpointRounding.AwayFromZero);

        AnsiConsole.MarkupLine($"\n[bold]Opciones de equipaje y tarifa para tu vuelo de {legTitle}[/] [grey]{Markup.Escape(policy.SubtitleLine)}[/]");
        var expl = ClientFareBundleDisplayDefaults.ApplyPricePlaceholders(policy.ExplainerLine, refCarryOn, refChecked, policy.SeatSelectionFromCop);
        AnsiConsole.MarkupLine($"[grey]{expl}[/]");
        WriteFareBundleComparisonCards(policy, refCarryOn, refChecked, pBasic, pClassic, pFlex);

        var pick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\n[bold]Confirmá tu tarifa:[/]")
                .PageSize(5)
                .AddChoices(
                    $"Basic — {FormatPriceCopColombia(pBasicTotal)} (pasaje + equipaje ref.)",
                    $"Classic — {FormatPriceCopColombia(pClassic)}",
                    $"Flex — {FormatPriceCopColombia(pFlex)}",
                    "0. Cancelar y volver"));

        if (pick.StartsWith("0.", StringComparison.Ordinal))
            return null;
        if (pick.StartsWith("Basic", StringComparison.Ordinal))
            return (
                $"{legTitle} Basic ref total {FormatPriceCopColombia(pBasicTotal)} (pasaje {FormatPriceCopColombia(pBasic)} + equipaje mano/bodega {FormatPriceCopColombia(basicBaggageRef)}); bolso; 3mi/USD; extras asiento/menú pago.",
                pBasicTotal,
                "Basic");
        if (pick.StartsWith("Classic", StringComparison.Ordinal))
            return ($"{legTitle} Classic ref {FormatPriceCopColombia(pClassic)}; bolso+mano+bodega; Eco; 6mi/USD;", pClassic, "Classic");
        if (pick.StartsWith("Flex", StringComparison.Ordinal))
            return ($"{legTitle} Flex ref {FormatPriceCopColombia(pFlex)}; Plus; cambios/reembolso; 8mi/USD;", pFlex, "Flex");
        return null;
    }

    /// <summary>Selector Basic/Classic/Flex por tramo (ida o vuelta). Devuelve texto para observaciones de la reserva o null si cancela.</summary>
    private static async Task<string?> PromptFareBundleSelectionForLegClientAsync(decimal basePrice, string legTitle) =>
        (await PromptFareBundleSelectionForLegClientWithPriceAsync(basePrice, legTitle))?.Observation;

    private sealed record RoundTripLegPick(
        int FlightId,
        DateTime FlightDateTime,
        string BundledObservation,
        decimal ReferencePricePerPerson,
        string FareTierName,
        Flight Flight,
        string OriginIata,
        string DestIata,
        int OriginAirportId,
        int DestAirportId,
        string AirlineName,
        string BlockDurationLabel);

    private static string FormatSpanishFlightDateHeader(DateOnly date)
    {
        var cult = CultureInfo.GetCultureInfo("es-CO");
        var dt = date.ToDateTime(TimeOnly.MinValue);
        var dayName = cult.TextInfo.ToTitleCase(dt.ToString("dddd", cult));
        var rest = date.ToString("dd MMM yyyy", cult);
        return $"{dayName}, {rest}";
    }

    /// <summary>Encabezado corto tipo «lun, 05 oct» (referencia comercial).</summary>
    private static string FormatSpanishFlightDateHeaderShort(DateOnly date)
    {
        var cult = CultureInfo.GetCultureInfo("es-CO");
        var dt = date.ToDateTime(TimeOnly.MinValue);
        var abbr = cult.TextInfo.ToTitleCase(dt.ToString("ddd", cult).Trim());
        abbr = abbr.TrimEnd('.');
        var dayMonth = date.ToString("dd MMM", cult).Trim();
        return $"{abbr}, {dayMonth}";
    }

    private static string CityPairPlain(IReadOnlyDictionary<int, Airport> airportById, int originId, int destId)
    {
        if (!airportById.TryGetValue(originId, out var o) || !airportById.TryGetValue(destId, out var d))
            return "? a ?";
        return $"{o.Name.Value} a {d.Name.Value}";
    }

    private static Panel BuildRoundTripLegSummaryPanel(string legLabel, RoundTripLegPick p, IReadOnlyDictionary<int, Airport> airportById)
    {
        var dep = p.Flight.DepartureTime.Value.ToString("HH:mm", CultureInfo.InvariantCulture);
        var arr = p.Flight.ArrivalTime.Value.ToString("HH:mm", CultureInfo.InvariantCulture);
        var dateShort = FormatSpanishFlightDateHeaderShort(p.Flight.Date.Value);
        var citiesPlain = CityPairPlain(airportById, p.OriginAirportId, p.DestAirportId);
        var headerShort = $"{legLabel} · {Markup.Escape(p.OriginIata)}→{Markup.Escape(p.DestIata)}";
        var dots = new string('·', 6) + " ✈ " + new string('·', 6);
        var origin = Markup.Escape(p.OriginIata);
        var dest = Markup.Escape(p.DestIata);
        var flightNo = Markup.Escape(p.Flight.Number.Value);
        var tierEsc = Markup.Escape(p.FareTierName);
        var durationEsc = Markup.Escape(p.BlockDurationLabel);
        var airlineEsc = Markup.Escape(p.AirlineName);
        var citiesEsc = Markup.Escape(citiesPlain);
        var badge = $"[bold white on red]{tierEsc}[/]";
        var body =
            $"[grey]{Markup.Escape(dateShort)}[/]\n" +
            $"[dim]{citiesEsc}[/]\n\n" +
            $"[bold]{dep}[/]  [grey]desde[/]  [bold]{origin}[/]  [grey]{dots}[/]  [bold]{arr}[/]  [grey]en[/]  [bold]{dest}[/]\n" +
            $"[grey]Vuelo directo[/]  ·  [dim]{durationEsc}[/]\n\n" +
            $"{badge}   [grey]·[/]   [dim]Vuelo[/] [bold]{flightNo}[/]\n" +
            $"[dim]Operado por {airlineEsc}[/]\n" +
            $"[grey]Referencia por persona:[/] [bold]{Markup.Escape(FormatPriceCopColombia(p.ReferencePricePerPerson))}[/]";
        return new Panel(new Markup(body))
            .Header(headerShort, Justify.Left)
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(Color.Green1))
            .Expand()
            .Padding(1, 1);
    }

    private sealed record RoundTripLegPostBookingDetail(string MarkupBody, decimal SumSeatsAddon);

    private static async Task<RoundTripLegPostBookingDetail> LoadRoundTripLegPostBookingDetailAsync(
        AppDbContext context,
        int idBooking,
        CancellationToken ct)
    {
        Booking booking;
        try
        {
            booking = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(idBooking, ct);
        }
        catch (KeyNotFoundException)
        {
            return new RoundTripLegPostBookingDetail("[red]No se encontró la información de la reserva de este tramo.[/]", 0m);
        }

        var allTickets = await new GetAllTicketsUseCase(new TicketRepository(context)).ExecuteAsync(ct);
        var ticket = allTickets.FirstOrDefault(t => t.IdBooking == idBooking);
        var bookingCodeEsc = Markup.Escape(booking.Code.Value);
        var ticketBlock = ticket is null
            ? "[yellow]Tiquete: (no registrado aún en listado)[/]"
            : $"Tiquete: [bold]{Markup.Escape(ticket.Code.Value)}[/]";

        var links = (await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct))
            .Where(l => l.IdBooking == idBooking)
            .OrderBy(l => l.IdSeat)
            .ToList();
        var allSeats = await new GetAllSeatsUseCase(new SeatRepository(context)).ExecuteAsync(ct);
        var seatById = allSeats.ToDictionary(s => s.Id.Value);

        var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(booking.IdFlight, ct);
        Fare? fare = null;
        Dictionary<int, decimal>? pricesByClass = null;
        if (flight.IdFare is int fid && fid > 0)
        {
            try
            {
                fare = await new GetFareByIdUseCase(new FareRepository(context)).ExecuteAsync(fid, ct);
            }
            catch (KeyNotFoundException)
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

        var seatClasses = await new GetAllSeatClassesUseCase(new SeatClassRepository(context)).ExecuteAsync(ct);
        var classNames = seatClasses.ToDictionary(sc => sc.Id.Value, sc => sc.Name.Value);
        var refEcon = fare is not null
            ? FareSeatClassPricingHelper.GetReferenceEconomyPrice(fare, pricesByClass, classNames)
            : 0m;

        var nums = new List<string>();
        decimal sumAddon = 0m;
        foreach (var link in links)
        {
            if (!seatById.TryGetValue(link.IdSeat, out var seat))
                continue;
            nums.Add(seat.Number.Value);
            if (fare is null)
                continue;
            var tp = FareSeatClassPricingHelper.GetSeatClassTotalPrice(fare, pricesByClass, seat.IdClase);
            sumAddon += Math.Max(0m, decimal.Round(tp - refEcon, 0, MidpointRounding.AwayFromZero));
        }

        sumAddon = decimal.Round(sumAddon, 0, MidpointRounding.AwayFromZero);

        var seatsJoined = nums.Count > 0
            ? string.Join(", ", nums.Select(Markup.Escape))
            : "[dim]—[/]";
        var sumAStr = Markup.Escape(FareSeatClassPricingHelper.FormatPriceCopColombia(sumAddon));

        var body =
            $"Reserva: [bold]{bookingCodeEsc}[/]\n" +
            $"{ticketBlock}\n" +
            $"[grey]Asientos elegidos[/] ([bold]{nums.Count}[/]): {seatsJoined}\n" +
            $"[grey]Extra por cabina (solo si es más cara que económica en este vuelo):[/] [bold]{sumAStr}[/]";

        return new RoundTripLegPostBookingDetail(body, sumAddon);
    }

    private static async Task WriteRoundTripTravelSummaryAfterBookingsAsync(
        RoundTripLegPick ida,
        RoundTripLegPick vue,
        int seatCount,
        IReadOnlyDictionary<int, Airport> airportById,
        int idaBookingId,
        int vueBookingId,
        CancellationToken ct)
    {
        Console.Clear();

        using var context = DbContextFactory.Create();
        var idaPost = await LoadRoundTripLegPostBookingDetailAsync(context, idaBookingId, ct);
        var vuePost = await LoadRoundTripLegPostBookingDetailAsync(context, vueBookingId, ct);

        var refTrip = decimal.Round(
            (ida.ReferencePricePerPerson + vue.ReferencePricePerPerson) * seatCount,
            0,
            MidpointRounding.AwayFromZero);
        var addonCabina = decimal.Round(idaPost.SumSeatsAddon + vuePost.SumSeatsAddon, 0, MidpointRounding.AwayFromZero);
        var servicios = 0m;
        var totalEstimado = decimal.Round(refTrip + addonCabina, 0, MidpointRounding.AwayFromZero);
        var impuestosTasas = addonCabina;

        AnsiConsole.Write(new Rule("[bold green]Resumen de compra[/]").Centered());
        var purchase = new Table().Border(TableBorder.Simple).Expand();
        purchase.AddColumn(new TableColumn(string.Empty).PadRight(1));
        purchase.AddColumn(new TableColumn(string.Empty).RightAligned());
        purchase.AddRow(
            new Markup("Precio de tiquetes"),
            new Markup(Markup.Escape(FormatPriceCopColombia(refTrip))));
        purchase.AddRow(
            new Markup("Impuestos, tasas y cargos"),
            new Markup(Markup.Escape(FormatPriceCopColombia(impuestosTasas))));
        purchase.AddRow(
            new Markup("Precio de servicios"),
            new Markup(Markup.Escape(FormatPriceCopColombia(servicios))));
        purchase.AddRow(
            new Markup("[bold]Total[/]"),
            new Markup($"[bold]{Markup.Escape(FormatPriceCopColombia(totalEstimado))}[/]"));
        AnsiConsole.Write(purchase);
        AnsiConsole.MarkupLine("[italic grey]*Incluye impuestos, recargos y servicios.*[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.Write(new Rule("[bold green]Tu selección[/]").Centered());
        var row = new Table { ShowHeaders = false, Expand = true };
        row.Border(TableBorder.None);
        row.AddColumn(new TableColumn(string.Empty).PadRight(1));
        row.AddColumn(new TableColumn(string.Empty).PadRight(1));
        row.AddRow(
            BuildRoundTripLegSummaryPanel("Vuelo de salida", ida, airportById),
            BuildRoundTripLegSummaryPanel("Vuelo de regreso", vue, airportById));
        AnsiConsole.Write(row);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[grey]Detalle de reserva, tiquete y asientos[/]");
        var idaSeatPanel = new Panel(new Markup(idaPost.MarkupBody))
            .Header("[bold]Ida[/]", Justify.Left)
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(Color.Green1))
            .Padding(1, 1);
        var vueSeatPanel = new Panel(new Markup(vuePost.MarkupBody))
            .Header("[bold]Vuelta[/]", Justify.Left)
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(Color.Green1))
            .Padding(1, 1);

        var seatRow = new Table { ShowHeaders = false, Expand = true };
        seatRow.Border(TableBorder.None);
        seatRow.AddColumn(new TableColumn(string.Empty).PadRight(1));
        seatRow.AddColumn(new TableColumn(string.Empty).PadRight(1));
        seatRow.AddRow(idaSeatPanel, vueSeatPanel);
        AnsiConsole.Write(seatRow);

        ConsolaPausa.PresionarCualquierTecla();
    }

    /// <summary>Solo selección de vuelo + tarifa para ida/vuelta; no crea la reserva.</summary>
    private static async Task<RoundTripLegPick?> TryPickRoundTripLegAsync(
        List<Flight> legList,
        string legTitle,
        int seatCount,
        CancellationToken ct,
        IReadOnlyList<FlightSearchOffer>? pricingOffers,
        IReadOnlyDictionary<int, Route> routeById)
    {
        var choices = legList
            .OrderBy(f => f.Id.Value)
            .Select(f =>
                $"{f.Id.Value}. {f.Number.Value} — {f.Date.Value:yyyy-MM-dd} {f.DepartureTime.Value:HH\\:mm} (cupo {FormatFlightCupoVentaVsCapacidad(f)} venta/cap.)")
            .ToList();
        choices.Add("0. Cancelar");

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"\nSelecciona tu vuelo de {legTitle}:")
                .PageSize(12)
                .AddChoices(choices));

        if (selected == "0. Cancelar")
        {
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return null;
        }

        var id = int.Parse(selected.Split('.')[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
        var flight = legList.First(f => f.Id.Value == id);
        var flightDateTime = flight.Date.Value.ToDateTime(flight.DepartureTime.Value);

        if (flight.AvailableSeats.Value < seatCount)
        {
            AnsiConsole.MarkupLine(
                $"\n[red]Este vuelo de {legTitle} no tiene cupo para las {seatCount} persona(s) de la búsqueda. " +
                $"Cupo a la venta ahora: [bold]{flight.AvailableSeats.Value}[/] de [bold]{flight.TotalCapacity.Value}[/].[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return null;
        }

        if (!routeById.TryGetValue(flight.IdRoute, out var route))
        {
            AnsiConsole.MarkupLine("\n[red]No se encontró la ruta de este vuelo en el catálogo.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return null;
        }

        var offer = pricingOffers?.FirstOrDefault(o => o.Flight.Id.Value == id);
        var originIata = offer?.OriginIata ?? "?";
        var destIata = offer?.DestIata ?? "?";
        var airlineName = offer?.AirlineName ?? "—";
        var blockDuration = offer?.BlockDurationLabel
            ?? FormatFlightDurationLineDisplay(flight.DepartureTime.Value, flight.ArrivalTime.Value);

        string bundledObservation;
        decimal refPrice;
        string fareTier;
        if (AppState.IdUserRole == 1)
        {
            bundledObservation = $"{legTitle} (admin) sin tarifa de referencia en búsqueda.";
            refPrice = 0m;
            fareTier = "—";
        }
        else if (legTitle is "IDA" or "VUELTA")
        {
            const decimal bundleReferenceWithoutPublishedFare = 250_000m;
            var hasPublishedFare = offer?.MinPrice is decimal;
            var baseForBundle = hasPublishedFare ? offer!.MinPrice!.Value : bundleReferenceWithoutPublishedFare;
            if (!hasPublishedFare)
            {
                AnsiConsole.MarkupLine(
                    "[grey]No hay tarifa publicada para este vuelo en esta fecha; igual podés elegir tipo de equipaje/tarifa (Basic/Classic/Flex) con montos de referencia.[/]");
            }

            AnsiConsole.WriteLine();
            var farePick = await PromptFareBundleSelectionForLegClientWithPriceAsync(baseForBundle, legTitle);
            if (farePick is null)
            {
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
                return null;
            }

            bundledObservation = farePick.Value.Observation;
            refPrice = farePick.Value.PricePerPerson;
            fareTier = farePick.Value.TierName;
            if (!hasPublishedFare)
                bundledObservation += " [ref sin tarifa publicada en sistema]";
        }
        else
        {
            bundledObservation = legTitle;
            refPrice = 0m;
            fareTier = "—";
        }

        return new RoundTripLegPick(
            id,
            flightDateTime,
            bundledObservation,
            refPrice,
            fareTier,
            flight,
            originIata,
            destIata,
            route.OriginAirport,
            route.DestinationAirport,
            airlineName,
            blockDuration);
    }

    private sealed record FlightSearchOffer(
        Flight Flight,
        string OriginIata,
        string DestIata,
        int AirlineId,
        string AirlineName,
        string BlockDurationLabel,
        bool IsDirect,
        decimal? MinPrice,
        bool RouteIsActive);

    private static bool FareCoversDate(Fare fare, DateOnly flightDate)
    {
        if (!fare.Active)
            return false;
        if (flightDate < fare.ValidFrom.Value || flightDate > fare.ValidTo.Value)
            return false;
        if (fare.ExpirationDate.Value is DateOnly exp && flightDate > exp)
            return false;
        return true;
    }

    private static string FormatBlockDuration(TimeOnly dep, TimeOnly arr)
    {
        var depDt = DateTime.Today.Add(dep.ToTimeSpan());
        var arrDt = DateTime.Today.Add(arr.ToTimeSpan());
        if (arr <= dep)
            arrDt = arrDt.AddDays(1);
        var span = arrDt - depDt;
        return $"{(int)span.TotalHours}h {span.Minutes:D2}m";
    }

    private static string FormatFlightCupoVentaVsCapacidad(Flight f) =>
        $"{f.AvailableSeats.Value}/{f.TotalCapacity.Value}";

    private static IEnumerable<int> FareIdsFromFlightsForPricing(IEnumerable<Flight> legFlights) =>
        legFlights.Select(f => f.IdFare).OfType<int>().Where(id => id > 0);

    private static List<FlightSearchOffer> BuildFlightOffers(
        List<Flight> legFlights,
        DateOnly legDate,
        Dictionary<int, Route> routeById,
        Dictionary<int, Airport> airportById,
        Dictionary<int, AirAggregate> aircraftById,
        Dictionary<int, string> airlineNameById,
        IReadOnlyList<Fare> fares,
        Dictionary<int, Dictionary<int, decimal>> seatClassPricesByFare)
    {
        var list = new List<FlightSearchOffer>(legFlights.Count);
        var fareById = fares.ToDictionary(f => f.Id.Value);
        foreach (var f in legFlights)
        {
            if (!routeById.TryGetValue(f.IdRoute, out var route))
                continue;
            var originIata = airportById.TryGetValue(route.OriginAirport, out var apO) ? apO.IATACode.Value : "?";
            var destIata = airportById.TryGetValue(route.DestinationAirport, out var apD) ? apD.IATACode.Value : "?";
            if (!aircraftById.TryGetValue(f.IdAircraft, out var ac))
            {
                var durationUnknown = FormatBlockDuration(f.DepartureTime.Value, f.ArrivalTime.Value);
                list.Add(new FlightSearchOffer(
                    f, originIata, destIata, 0, "(Aeronave no encontrada en catálogo)", durationUnknown, true, null, route.Active));
                continue;
            }

            var airlineId = ac.IdAirline;
            var airlineName = airlineNameById.TryGetValue(airlineId, out var an) ? an : $"Aerolínea {airlineId}";
            // «Desde»: mínimo entre precios por clase (FareSeatClassPrice) si existen; si no, precio base de la tarifa del vuelo.
            decimal? minPrice = null;
            if (f.IdFare is int fareId && fareId > 0 && fareById.TryGetValue(fareId, out var fare))
            {
                if (FareCoversDate(fare, legDate))
                {
                    if (seatClassPricesByFare.TryGetValue(fareId, out var byClass) && byClass.Count > 0)
                        minPrice = byClass.Values.Min();
                    else
                        minPrice = fare.BasePrice.Value;
                }
            }
            var duration = FormatBlockDuration(f.DepartureTime.Value, f.ArrivalTime.Value);
            list.Add(new FlightSearchOffer(f, originIata, destIata, airlineId, airlineName, duration, true, minPrice, route.Active));
        }

        return list;
    }

    /// <summary>Duración tipo «1h 5m» (como en tarjetas de búsqueda comerciales).</summary>
    private static string FormatFlightDurationLineDisplay(TimeOnly dep, TimeOnly arr)
    {
        var depDt = DateTime.Today.Add(dep.ToTimeSpan());
        var arrDt = DateTime.Today.Add(arr.ToTimeSpan());
        if (arr <= dep)
            arrDt = arrDt.AddDays(1);
        var span = arrDt - depDt;
        return $"{(int)span.TotalHours}h {span.Minutes}m";
    }

    /// <summary>Precio en COP con separador de miles estilo Colombia (ej. $235.330 COP).</summary>
    private static string FormatPriceCopColombia(decimal amount)
    {
        var n = amount.ToString("N0", CultureInfo.GetCultureInfo("es-CO"))
            .Replace('\u00a0', ' ')
            .Trim();
        return "$" + n + " COP";
    }

    private static string BuildFlightOfferCardLeftMarkup(FlightSearchOffer o, string durationDisplay)
    {
        var f = o.Flight;
        var dep = f.DepartureTime.Value.ToString("HH:mm");
        var arr = f.ArrivalTime.Value.ToString("HH:mm");
        var directPart = o.IsDirect
            ? "[underline #0d9488]Directo[/]"
            : "[grey]Con escalas[/]";
        var line1 = $"[bold]{dep}[/]  {directPart} | [grey]{Markup.Escape(durationDisplay)}[/]  [bold]{arr}[/]";
        var dots = new string('·', 10) + " ✈ " + new string('·', 10);
        var line2 = $"[dim]{Markup.Escape(o.OriginIata)}[/]  [grey]{dots}[/]  [dim]{Markup.Escape(o.DestIata)}[/]";
        var line3 = $"[grey]Operado por {Markup.Escape(o.AirlineName)}[/]";
        var routeWarn = o.RouteIsActive
            ? string.Empty
            : "\n[yellow]Ruta inactiva en catálogo: si no podés completar la reserva, pedí que la reactiven (Vuelos → Rutas).[/]";
        return $"{line1}\n{line2}\n{line3}{routeWarn}";
    }

    private static string BuildFlightOfferCardRightMarkup(FlightSearchOffer o, bool isCheapestPrice)
    {
        if (o.MinPrice is not decimal p)
        {
            return "[grey]Desde[/]\n[bold]—[/]\n[dim]Sin tarifa para esta fecha[/]";
        }

        var priceTxt = Markup.Escape(FormatPriceCopColombia(p));
        if (isCheapestPrice)
            return "[bold white on green3] Mejor precio [/]\n[grey]Desde[/]\n[bold]" + priceTxt + "[/]";
        return "[grey]Desde[/]\n[bold]" + priceTxt + "[/]";
    }

    /// <summary>
    /// Si no hay vuelo exacto origen→destino pero sí salidas el mismo día desde el mismo origen hacia otro aeropuerto,
    /// lo indica (p. ej. dos filas en «Aeropuerto» con códigos MDE y MED).
    /// </summary>
    private static void WriteSameDayOtherDestinationsFromOrigin(
        IReadOnlyList<Flight> allFlights,
        IReadOnlyDictionary<int, Route> routeByIdDict,
        IReadOnlyDictionary<int, Airport> airportByIdDict,
        DateOnly legDate,
        int idOriginAirport,
        int idDestAirport)
    {
        var rows = allFlights
            .Where(f => f.Date.Value == legDate)
            .Select(f => (f, r: routeByIdDict.TryGetValue(f.IdRoute, out var rt) ? rt : null))
            .Where(x => x.r is { } route && route.OriginAirport == idOriginAirport && route.DestinationAirport != idDestAirport)
            .OrderBy(x => x.f.DepartureTime.Value)
            .ToList();
        if (rows.Count == 0)
            return;

        AnsiConsole.MarkupLine(
            "\n[grey]Aviso: no hay vuelo para el trayecto [bold]exacto[/] que buscás, pero [bold]el mismo día[/] sí hay " +
            "salida(s) desde el mismo origen hacia [bold]otro[/] aeropuerto. Revisá en [bold]Vuelos → Listar vuelos[/] el código IATA del destino del vuelo (p. ej. MED vs MDE).[/]");
        foreach (var (f, r) in rows)
        {
            var dIata = airportByIdDict.TryGetValue(r!.DestinationAirport, out var ad) ? ad.IATACode.Value : "?";
            AnsiConsole.MarkupLine(
                $"  [dim]· {Markup.Escape(f.Number.Value)} → {Markup.Escape(dIata)}[/] [grey]cupo[/] " +
                $"[bold]{f.AvailableSeats.Value}/{f.TotalCapacity.Value}[/] [grey](venta/capacidad)[/]");
        }
    }

    /// <summary>
    /// Si no existe ruta activa destino→origen exacta, lista rutas activas X→origen por si X es otro aeropuerto en catálogo.
    /// </summary>
    private static void WriteReturnRouteAlternativesHint(
        IReadOnlyList<Route> routes,
        IReadOnlyDictionary<int, Airport> airportByIdDict,
        int idOriginAirport,
        int idDestAirport,
        string originIata,
        string destIata)
    {
        var toHome = routes
            .Where(r => r.Active && r.DestinationAirport == idOriginAirport && r.OriginAirport != idDestAirport)
            .ToList();
        if (toHome.Count == 0)
            return;

        AnsiConsole.MarkupLine(
            $"[grey]Rutas [bold]activas[/] que llegan a {Markup.Escape(originIata)} pero salen de otro aeropuerto (no {Markup.Escape(destIata)}→{Markup.Escape(originIata)}):[/]");
        foreach (var r in toHome.OrderBy(x => x.Id.Value))
        {
            var fromIata = airportByIdDict.TryGetValue(r.OriginAirport, out var a) ? a.IATACode.Value : "?";
            AnsiConsole.MarkupLine($"  [dim]· Ruta {r.Id.Value}: {Markup.Escape(fromIata)}→{Markup.Escape(originIata)}[/]");
        }
    }

    /// <summary>
    /// Para la vuelta: si no hay vuelo destino→origen pero sí hay otro→origen el mismo día.
    /// </summary>
    private static void WriteSameDayFlightsToOriginFromOtherAirports(
        IReadOnlyList<Flight> allFlights,
        IReadOnlyDictionary<int, Route> routeByIdDict,
        IReadOnlyDictionary<int, Airport> airportByIdDict,
        DateOnly legDate,
        int idOriginAirport,
        int idExpectedDepartureAirport)
    {
        var rows = allFlights
            .Where(f => f.Date.Value == legDate)
            .Select(f => (f, r: routeByIdDict.TryGetValue(f.IdRoute, out var rt) ? rt : null))
            .Where(x => x.r is { } route && route.DestinationAirport == idOriginAirport && route.OriginAirport != idExpectedDepartureAirport)
            .OrderBy(x => x.f.DepartureTime.Value)
            .ToList();
        if (rows.Count == 0)
            return;

        var expectFrom = airportByIdDict.TryGetValue(idExpectedDepartureAirport, out var depAp) ? depAp.IATACode.Value : "?";
        var toIata = airportByIdDict.TryGetValue(idOriginAirport, out var oAp) ? oAp.IATACode.Value : "?";
        AnsiConsole.MarkupLine(
            $"\n[grey]Ese día hay vuelo(s) que llegan a {Markup.Escape(toIata)} pero salen de [bold]otro[/] aeropuerto " +
            $"(no desde {Markup.Escape(expectFrom)}, el destino de tu ida). Revisá listar vuelos / aeropuertos en catálogo.[/]");
        foreach (var (f, r) in rows)
        {
            var fromIata = airportByIdDict.TryGetValue(r!.OriginAirport, out var a) ? a.IATACode.Value : "?";
            AnsiConsole.MarkupLine(
                $"  [dim]· {Markup.Escape(f.Number.Value)} {Markup.Escape(fromIata)}→{Markup.Escape(toIata)}[/] [grey]cupo[/] " +
                $"[bold]{f.AvailableSeats.Value}/{f.TotalCapacity.Value}[/]");
        }
    }

    private static void WriteFlightOffersSection(
        string title,
        DateOnly requestedDate,
        IReadOnlyList<Flight> flightsOnDate,
        int flightsWithEnoughCupo,
        List<FlightSearchOffer> offers,
        int passengersRequested)
    {
        AnsiConsole.MarkupLine($"[cyan]{Markup.Escape(title)}[/] [grey](fecha: {requestedDate:yyyy-MM-dd})[/]");
        if (offers.Count > 0)
        {
            AnsiConsole.MarkupLine(
                "[dim]Todos los vuelos del día en este trayecto. Al reservar se valida que el cupo alcance para las personas que indicaste en la búsqueda.[/]");
            var priced = offers.Where(o => o.MinPrice.HasValue).ToList();
            decimal? globalMin = priced.Count > 0 ? priced.Min(x => x.MinPrice!.Value) : null;

            foreach (var o in offers.OrderBy(x => x.Flight.DepartureTime.Value))
            {
                var f = o.Flight;
                var durationDisplay = FormatFlightDurationLineDisplay(f.DepartureTime.Value, f.ArrivalTime.Value);
                var isCheapest = globalMin is decimal gm && o.MinPrice is decimal mp && mp == gm;
                var left = BuildFlightOfferCardLeftMarkup(o, durationDisplay);
                var right = BuildFlightOfferCardRightMarkup(o, isCheapest);

                var innerTitle =
                    $"Vuelo {f.Number.Value} · ID {f.Id.Value} · cupo {FormatFlightCupoVentaVsCapacidad(f)} (venta/capacidad)";
                if (!o.RouteIsActive)
                    innerTitle += " — ruta inactiva";
                if (f.AvailableSeats.Value < passengersRequested)
                    innerTitle += $" — cupo insuficiente para {passengersRequested} persona(s)";

                var card = new Table { ShowHeaders = false };
                card.Border(TableBorder.Rounded);
                card.BorderStyle(new Style(Color.Grey));
                card.Title = new TableTitle("[grey]" + Markup.Escape(innerTitle) + "[/]");
                card.AddColumn(new TableColumn(string.Empty).Width(52));
                card.AddColumn(new TableColumn(string.Empty).Width(26).RightAligned());
                card.AddRow(new Markup(left), new Markup(right));
                AnsiConsole.Write(card);
                AnsiConsole.WriteLine();
            }

            return;
        }

        if (flightsOnDate.Count == 0)
        {
            AnsiConsole.MarkupLine(
                "[yellow]  No hay vuelos en esa fecha para este trayecto (origen→destino).[/]");
            AnsiConsole.MarkupLine(
                "[grey]  Comprobá en [bold]Vuelos → Listar vuelos[/] que la [bold]fecha[/] del vuelo sea exactamente la que buscás, " +
                "que el trayecto coincida con la ruta del vuelo y que el vuelo exista.[/]");
            return;
        }

        if (flightsWithEnoughCupo == 0)
        {
            var maxCupo = flightsOnDate.Max(f => f.AvailableSeats.Value);
            AnsiConsole.MarkupLine(
                "[yellow]  Hay vuelo(s) ese día, pero ninguno con cupo a la venta para [bold]" +
                passengersRequested + "[/] persona(s). Mayor cupo disponible: [bold]" + maxCupo + "[/].[/]");
            AnsiConsole.MarkupLine(
                "[grey]  [bold]venta/capacidad[/]: el primer número es lo que aún se puede reservar; baja con cada reserva. Probá con menos pasajeros u otra fecha.[/]");
            return;
        }

        AnsiConsole.MarkupLine(
            "[yellow]  Hay vuelo(s) con cupo suficiente, pero no se pudieron armar ofertas " +
            "(revisá ruta, aeropuertos y aeronave en catálogo).[/]");
    }

    private static async Task<int> SelectAirportAsync(string prompt, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var airports = await new GetAllAirportsUseCase(new AirportRepository(context)).ExecuteAsync(ct);
        if (!airports.Any()) throw new InvalidOperationException("No hay aeropuertos registrados. Un administrador debe crearlos primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(prompt)
                .PageSize(10)
                .AddChoices(airports.Select(a => $"{a.Id.Value}. {a.Name.Value} ({a.IATACode.Value})")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var flights = (await new GetAllFlightsUseCase(new FlightRepository(context)).ExecuteAsync(ct))
            .OrderBy(f => f.Id.Value)
            .ToList();
        var routes = await new GetAllRoutesUseCase(new RouteRepository(context)).ExecuteAsync(ct);
        var aircrafts = await new GetAllAircraftsUseCase(new AircraftRepository(context)).ExecuteAsync(ct);
        var fares = await new GetAllFaresUseCase(new FareRepository(context)).ExecuteAsync(ct);
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var airports = await new GetAllAirportsUseCase(new AirportRepository(context)).ExecuteAsync(ct);
        var crews = await new GetAllCrewsUseCase(new CrewRepository(context)).ExecuteAsync(ct);

        var iata = airports.ToDictionary(a => a.Id.Value, a => a.IATACode.Value);
        static string LegLabel(IReadOnlyDictionary<int, string> map, int originId, int destId) =>
            $"{(map.TryGetValue(originId, out var o) ? o : $"#{originId}")}→{(map.TryGetValue(destId, out var d) ? d : $"#{destId}")}";

        var routeById = routes.ToDictionary(r => r.Id.Value);
        var aircraftById = aircrafts.ToDictionary(a => a.Id.Value);
        var fareById = fares.ToDictionary(f => f.Id.Value);
        var statusMap = statuses.ToDictionary(s => s.Id.Value, s => s.Name.Value);
        var crewById = crews.ToDictionary(c => c.Id.Value, c => c.GroupName.Value);

        if (!flights.Any()) { AnsiConsole.MarkupLine("[yellow]No hay vuelos registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID");
            table.AddColumn("Número");
            table.AddColumn("Fecha");
            table.AddColumn("Salida");
            table.AddColumn("Llegada");
            table.AddColumn("Origen→Destino");
            table.AddColumn("Id ruta");
            table.AddColumn("Aeronave");
            table.AddColumn("Cupo venta/cap.");
            table.AddColumn("Tarifa");
            table.AddColumn("Precio");
            table.AddColumn("Estado");
            table.AddColumn("Tripulación");
            foreach (var f in flights)
            {
                string leg;
                string routeId;
                if (routeById.TryGetValue(f.IdRoute, out var r))
                {
                    leg = LegLabel(iata, r.OriginAirport, r.DestinationAirport);
                    routeId = r.Id.Value.ToString();
                }
                else
                {
                    leg = $"? (ruta #{f.IdRoute})";
                    routeId = f.IdRoute.ToString();
                }
                var aircraftCell = aircraftById.TryGetValue(f.IdAircraft, out var ac)
                    ? $"AC{ac.Id.Value} — aerolínea {ac.IdAirline}, cap. {ac.Capacity.Value}"
                    : $"Aeronave #{f.IdAircraft}";
                var fareCell = f.IdFare is int idFare
                    ? (fareById.TryGetValue(idFare, out var fare) ? $"{idFare}. {fare.Name.Value}" : $"Tarifa #{idFare}")
                    : "-";
                var priceCell = f.IdFare is int idFarePrice && fareById.TryGetValue(idFarePrice, out var farePrice)
                    ? FormatPriceCopColombia(farePrice.BasePrice.Value)
                    : "-";
                var status = statusMap.TryGetValue(f.IdStatus, out var sn) ? sn : f.IdStatus.ToString();
                var crewCell = crewById.TryGetValue(f.IdCrew, out var gn)
                    ? $"{f.IdCrew}. {gn}"
                    : $"Tripulación #{f.IdCrew}";
                table.AddRow(
                    f.Id.Value.ToString(),
                    Markup.Escape(f.Number.Value),
                    f.Date.Value.ToString("yyyy-MM-dd"),
                    f.DepartureTime.Value.ToString("HH:mm"),
                    f.ArrivalTime.Value.ToString("HH:mm"),
                    Markup.Escape(leg),
                    routeId,
                    Markup.Escape(aircraftCell),
                    $"{f.AvailableSeats.Value}/{f.TotalCapacity.Value}",
                    Markup.Escape(fareCell),
                    Markup.Escape(priceCell),
                    Markup.Escape(status),
                    Markup.Escape(crewCell));
            }
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task<int> SelectRouteAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var routes = await new GetAllRoutesUseCase(new RouteRepository(context)).ExecuteAsync(ct);
        var airports = await new GetAllAirportsUseCase(new AirportRepository(context)).ExecuteAsync(ct);
        var iata = airports.ToDictionary(a => a.Id.Value, a => a.IATACode.Value);
        static string LegLabel(IReadOnlyDictionary<int, string> map, int originId, int destId) =>
            $"{(map.TryGetValue(originId, out var o) ? o : $"#{originId}")}→{(map.TryGetValue(destId, out var d) ? d : $"#{destId}")}";

        var active = routes.Where(r => r.Active).ToList();
        if (!active.Any()) throw new InvalidOperationException("No hay rutas activas. Activa una ruta primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona la ruta (sentido origen→destino):").PageSize(12)
                .AddChoices(active
                    .OrderBy(r => r.Id.Value)
                    .Select(r =>
                        $"{r.Id.Value}. {LegLabel(iata, r.OriginAirport, r.DestinationAirport)} — {r.DistanceKm.Value:F0} km, {r.EstDuration.Value:HH\\:mm}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<(int idAircraft, int capacity, int airlineId)> SelectAircraftAsync(CancellationToken ct, int? restrictToAirlineId = null)
    {
        using var context = DbContextFactory.Create();
        var aircrafts = await new GetAllAircraftsUseCase(new AircraftRepository(context)).ExecuteAsync(ct);

        // En flujo admin, primero se elige la aerolínea para luego ver sus aeronaves y su capacidad/asientos.
        int airlineId;
        if (restrictToAirlineId is int presetAirlineId)
        {
            airlineId = presetAirlineId;
        }
        else
        {
            var aerolines = await new GetAllAerolinesUseCase(new AerolineRepository(context)).ExecuteAsync(ct);
            if (!aerolines.Any())
                throw new InvalidOperationException("No hay aerolíneas registradas. Registra una primero.");

            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecciona la aerolínea que operará el vuelo:")
                    .PageSize(12)
                    .AddChoices(aerolines.OrderBy(a => a.Id.Value).Select(a => $"{a.Id.Value}. {a.Name.Value} ({a.IATACode.Value})")));
            airlineId = int.Parse(pick.Split(new char[] { '.' })[0]);
        }

        var list = aircrafts.Where(a => a.IdAirline == airlineId).OrderBy(a => a.Id.Value).ToList();
        if (!list.Any())
        {
            throw new InvalidOperationException(
                $"No hay aeronaves registradas para la aerolínea (ID {airlineId}). Registra una aeronave para esa aerolínea primero.");
        }

        var seatCounts = await context.Set<SeatEntity>()
            .AsNoTracking()
            .Where(s => list.Select(a => a.Id.Value).Contains(s.IdAircraft))
            .GroupBy(s => s.IdAircraft)
            .Select(g => new { IdAircraft = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.IdAircraft, x => x.Count, ct);

        var title = restrictToAirlineId is int
            ? "Selecciona la aeronave (de la aerolínea filtrada):"
            : "Selecciona la aeronave:";

        // Mejor UX: primero aeronaves con asientos creados; además, mostrar “asientos creados vs capacidad”.
        string AircraftLabel(AirAggregate a)
        {
            var seats = seatCounts.TryGetValue(a.Id.Value, out var c) ? c : 0;
            var cap = a.Capacity.Value;

            string seatInfo;
            if (seats == 0)
            {
                seatInfo = "[red]sin asientos[/]";
            }
            else if (seats < cap)
            {
                seatInfo = $"[yellow]{seats} asientos[/] ([grey]faltan {cap - seats}[/])";
            }
            else
            {
                seatInfo = $"[green]{seats} asientos[/]";
            }

            return $"{a.Id.Value}. Aeronave {a.Id.Value} — capacidad {cap} — {seatInfo}";
        }

        var ordered = list
            .OrderByDescending(a => seatCounts.TryGetValue(a.Id.Value, out var c) && c > 0)
            .ThenByDescending(a => seatCounts.TryGetValue(a.Id.Value, out var c) ? c : 0)
            .ThenBy(a => a.Id.Value)
            .ToList();
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title(title).PageSize(12)
                .AddChoices(ordered.Select(AircraftLabel)));
        var id = int.Parse(selected.Split(new char[] { '.' })[0]);
        var aircraft = ordered.First(a => a.Id.Value == id);
        return (id, aircraft.Capacity.Value, aircraft.IdAirline);
    }

    /// <summary>
    /// Asegura que exista una tarifa activa para la aerolínea que cubra la fecha del vuelo.
    /// Si ya hay tarifas aplicables, permite seleccionar una o crear otra.
    /// Si no hay, obliga a crearla aquí (para que el vuelo quede publicado con “Desde” en búsqueda).
    /// </summary>
    private static async Task<int?> EnsureFareForAirlineAndDateAsync(int airlineId, int idAircraft, DateOnly flightDate, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var fareRepo = new FareRepository(context);
        var fares = await new GetAllFaresUseCase(fareRepo).ExecuteAsync(ct);
        var applicable = fares
            .Where(f => f.IdAirline == airlineId && FareCoversDate(f, flightDate))
            .OrderBy(f => f.BasePrice.Value)
            .ToList();

        var dateLabel = flightDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        AnsiConsole.MarkupLine($"\n[cyan]Tarifa para aerolínea {airlineId}[/] [grey](fecha vuelo: {dateLabel})[/]");

        // Si ya hay una tarifa aplicable, el admin puede dejarla o crear una nueva.
        if (applicable.Count > 0)
        {
            var choices = new List<string>();
            choices.AddRange(applicable.Select(f =>
                $"{f.Id.Value}. {f.Name.Value} — {FormatPriceCopColombia(f.BasePrice.Value)} (vigencia {f.ValidFrom.Value:yyyy-MM-dd}→{f.ValidTo.Value:yyyy-MM-dd})"));
            choices.Add("N. Crear nueva tarifa para esta aerolínea");
            choices.Add("0. Omitir (no recomendado)");

            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecciona la tarifa que aplicará al vuelo (para precios en búsqueda):")
                    .PageSize(12)
                    .AddChoices(choices));

            if (pick.StartsWith("0.", StringComparison.Ordinal))
            {
                AnsiConsole.MarkupLine("[yellow]Aviso:[/] continuarás sin tarifa nueva; el cliente puede ver “Sin tarifa” en búsqueda según la fecha.");
                return null;
            }

            if (!pick.StartsWith("N.", StringComparison.Ordinal))
            {
                // Seleccionó una existente. No hay nada que persistir.
                return int.Parse(pick.Split(new char[] { '.' })[0]);
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]No hay tarifas activas[/] que cubran esta fecha para esta aerolínea.");
            AnsiConsole.MarkupLine("[grey]Para que el vuelo muestre precio en «Buscar vuelos», crea la tarifa ahora.[/]");
        }

        // Crear nueva tarifa (con precios por clase del avión)
        var nameDefault = $"Tarifa {airlineId} {flightDate:yyyyMM}";
        var name = AnsiConsole.Prompt(new TextPrompt<string>($"Nombre de la tarifa (Enter = {Markup.Escape(nameDefault)}):")
            .DefaultValue(nameDefault));

        // Detectar clases reales del avión según sus asientos.
        async Task<List<(int IdClase, int Count)>> LoadSeatGroupsAsync()
        {
            var groups = await context.Set<SeatEntity>()
                .AsNoTracking()
                .Where(s => s.IdAircraft == idAircraft)
                .GroupBy(s => s.IdClase)
                .Select(g => new { IdClase = g.Key, Count = g.Count() })
                .ToListAsync(ct);
            return groups.Select(x => (x.IdClase, x.Count)).ToList();
        }

        var seatGroups = await LoadSeatGroupsAsync();
        if (seatGroups.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]La aeronave seleccionada no tiene asientos registrados.[/]");
            if (AnsiConsole.Confirm("¿Deseas generarlos automáticamente ahora para poder asignar precios por clase?", true))
            {
                var aircraftCapacity = await context.Set<AircraftEntity>()
                    .AsNoTracking()
                    .Where(a => a.IdAircraft == idAircraft)
                    .Select(a => a.Capacity)
                    .FirstOrDefaultAsync(ct);

                if (aircraftCapacity <= 0)
                    throw new InvalidOperationException("No se pudo obtener la capacidad de la aeronave para generar asientos.");

                // Reutiliza el mismo generador de asientos del módulo de vuelos
                await GenerateAircraftSeatsAsync(idAircraft, aircraftCapacity, ct);

                // Recargar grupos de asientos luego de generar
                seatGroups = await LoadSeatGroupsAsync();
            }
            else
            {
                // Alternativa: permitir crear tarifa simple sin precios por clase
                var baseOnly = AnsiConsole.Prompt(
                    new TextPrompt<decimal>("Precio base de la tarifa (COP):")
                        .Validate(v => v > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Debe ser mayor a 0[/]")));

                var activeBaseOnly = AnsiConsole.Confirm("¿Tarifa activa?", true);
                await new CreateFareUseCase(fareRepo).ExecuteAsync(name, baseOnly, flightDate, flightDate, null, airlineId, activeBaseOnly, ct);
                await context.SaveChangesAsync(ct);
                AnsiConsole.MarkupLine("[green]Tarifa creada (precio base) sin desglose por clase.[/]");
                var createdIdBase = await context.Set<FareEntity>()
                    .AsNoTracking()
                    .Where(f =>
                        f.FareName == name &&
                        f.BasePrice == baseOnly &&
                        f.ValidFrom == flightDate &&
                        f.ValidTo == flightDate &&
                        f.IdAirline == airlineId &&
                        f.Active == activeBaseOnly &&
                        f.ExpirationDate == null)
                    .OrderByDescending(f => f.IdFare)
                    .Select(f => f.IdFare)
                    .FirstOrDefaultAsync(ct);
                return createdIdBase > 0 ? createdIdBase : null;
            }

            if (seatGroups.Count == 0)
                throw new InvalidOperationException("No se pudieron detectar asientos para la aeronave. Crea/genera asientos primero.");
        }

        var seatClassNames = await context.Set<SeatClassEntity>()
            .AsNoTracking()
            .Where(sc => seatGroups.Select(x => x.IdClase).Contains(sc.IdClase))
            .ToDictionaryAsync(sc => sc.IdClase, sc => sc.ClassName, ct);

        AnsiConsole.MarkupLine("\n[grey]Define el precio por [bold]clase[/] según las clases presentes en esta aeronave.[/]");
        var classPrices = new List<(int idClase, decimal price)>(seatGroups.Count);
        foreach (var g in seatGroups.OrderBy(x => x.IdClase))
        {
            var clsName = seatClassNames.TryGetValue(g.IdClase, out var n) ? n : $"Clase {g.IdClase}";
            var price = AnsiConsole.Prompt(
                new TextPrompt<decimal>($"Precio por asiento — {Markup.Escape(clsName)} ({g.Count} asiento(s) en el avión):")
                    .Validate(v => v > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Debe ser mayor a 0[/]")));
            classPrices.Add((g.IdClase, price));
        }

        // Para mantener compatibilidad con el resto del sistema (búsqueda “Desde”),
        // guardamos BasePrice = mínimo entre las clases.
        var basePrice = classPrices.Min(x => x.price);

        // Por defecto la tarifa cubre solo ese día (válido para el modelo actual).
        var validFrom = flightDate;
        var validTo = flightDate;
        if (AnsiConsole.Confirm("¿Quieres que la tarifa tenga un rango de vigencia mayor a 1 día?", false))
        {
            var fromStr = AnsiConsole.Prompt(
                new TextPrompt<string>($"Vigencia desde (yyyy-MM-dd, Enter = {flightDate:yyyy-MM-dd}):")
                    .DefaultValue(flightDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                    .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Formato inválido[/]")));
            var toStr = AnsiConsole.Prompt(
                new TextPrompt<string>($"Vigencia hasta (yyyy-MM-dd, Enter = {flightDate:yyyy-MM-dd}):")
                    .DefaultValue(flightDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                    .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Formato inválido[/]")));
            validFrom = DateOnly.ParseExact(fromStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            validTo = DateOnly.ParseExact(toStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (validTo < validFrom)
                throw new InvalidOperationException("La vigencia hasta no puede ser menor que la vigencia desde.");
            if (flightDate < validFrom || flightDate > validTo)
                throw new InvalidOperationException("La fecha del vuelo debe quedar dentro de la vigencia de la tarifa.");
        }

        DateOnly? expiration = null;
        if (AnsiConsole.Confirm("¿La tarifa tiene fecha de vencimiento (expiration) opcional?", false))
        {
            var expStr = AnsiConsole.Prompt(
                new TextPrompt<string>("Expiration (yyyy-MM-dd):")
                    .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Formato inválido[/]")));
            expiration = DateOnly.ParseExact(expStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        var active = AnsiConsole.Confirm("¿Tarifa activa?", true);
        await new CreateFareUseCase(fareRepo).ExecuteAsync(name, basePrice, validFrom, validTo, expiration, airlineId, active, ct);
        await context.SaveChangesAsync(ct);

        // Recuperar IdFare real y guardar el desglose por clase.
        var idFare = await context.Set<FareEntity>()
            .AsNoTracking()
            .Where(f =>
                f.FareName == name &&
                f.BasePrice == basePrice &&
                f.ValidFrom == validFrom &&
                f.ValidTo == validTo &&
                f.IdAirline == airlineId &&
                f.Active == active &&
                f.ExpirationDate == expiration)
            .OrderByDescending(f => f.IdFare)
            .Select(f => f.IdFare)
            .FirstOrDefaultAsync(ct);

        if (idFare <= 0)
            throw new InvalidOperationException("No se pudo recuperar el ID de la tarifa recién creada para guardar precios por clase.");

        var rows = classPrices.Select(cp => new FareSeatClassPriceEntity
        {
            IdFare = idFare,
            IdClase = cp.idClase,
            Price = cp.price
        }).ToList();
        await context.Set<FareSeatClassPriceEntity>().AddRangeAsync(rows, ct);
        await context.SaveChangesAsync(ct);

        AnsiConsole.MarkupLine("[green]Tarifa creada con precios por clase y guardada.[/]");
        return idFare;
    }

    private static async Task<int> SelectCrewAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var crews = await new GetAllCrewsUseCase(new CrewRepository(context)).ExecuteAsync(ct);
        if (!crews.Any()) throw new InvalidOperationException("No hay grupos de tripulación registrados. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona la tripulación:").PageSize(10)
                .AddChoices(crews.Select(c => $"{c.Id.Value}. {c.GroupName.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectStatusAsync(string title, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var flightStatuses = statuses.Where(s => s.EntityType.Value == "Flight").ToList();
        if (!flightStatuses.Any()) throw new InvalidOperationException("No hay estados de vuelo. Crea uno en Administración.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title(title).PageSize(10)
                .AddChoices(flightStatuses.Select(s => $"{s.Id.Value}. {s.Name.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    /// <summary>Obtiene el ID de una ruta activa origen→destino o la crea con los datos que indique el administrador.</summary>
    private static async Task<int> EnsureActiveRouteAsync(int originAirportId, int destAirportId, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var routeRepo = new RouteRepository(context);
        var routes = await new GetAllRoutesUseCase(routeRepo).ExecuteAsync(ct);
        var hit = routes.FirstOrDefault(r => r.Active && r.OriginAirport == originAirportId && r.DestinationAirport == destAirportId);
        if (hit is not null)
            return hit.Id.Value;

        var airports = await new GetAllAirportsUseCase(new AirportRepository(context)).ExecuteAsync(ct);
        string ApName(int id) =>
            airports.FirstOrDefault(a => a.Id.Value == id) is { } a ? $"{a.Name.Value} ({a.IATACode.Value})" : $"#{id}";

        AnsiConsole.MarkupLine(
            $"\n[yellow]Falta una ruta activa {Markup.Escape(ApName(originAirportId))} → {Markup.Escape(ApName(destAirportId))}. Definila ahora.[/]");

        var distance = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Distancia en km (ej: 850.5):")
                .Validate(v => v > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]La distancia debe ser mayor a 0[/]")));
        var durationStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Duración estimada (HH:mm, ej: 01:45):")
                .Validate(s => TimeOnly.TryParseExact(s, "HH:mm", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido. Use HH:MM[/]")));
        var duration = TimeOnly.ParseExact(durationStr, "HH:mm");
        var active = AnsiConsole.Confirm("¿Ruta activa?", true);

        var created = await new CreateRouteUseCase(routeRepo).ExecuteAsync(distance, duration, originAirportId, destAirportId, active, ct);
        await context.SaveChangesAsync(ct);

        var createdId = await context.Set<RouteEntity>()
            .AsNoTracking()
            .Where(r =>
                r.OriginAirport == originAirportId &&
                r.DestinationAirport == destAirportId &&
                r.DistanceKm == created.DistanceKm.Value &&
                r.EstDuration == created.EstDuration.Value &&
                r.Active == active)
            .OrderByDescending(r => r.IdRoute)
            .Select(r => r.IdRoute)
            .FirstAsync(ct);

        AnsiConsole.MarkupLine($"[green]Ruta registrada con ID {createdId}.[/]");
        return createdId;
    }

    private static async Task CreateRoundTripFromExistingRoutesAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR VUELOS — IDA Y VUELTA[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear dos vuelos (ida + regreso) usando rutas existentes?", true))
            return;

        using var preCtx = DbContextFactory.Create();
        var routes = await new GetAllRoutesUseCase(new RouteRepository(preCtx)).ExecuteAsync(ct);
        var airports = await new GetAllAirportsUseCase(new AirportRepository(preCtx)).ExecuteAsync(ct);
        var iata = airports.ToDictionary(a => a.Id.Value, a => a.IATACode.Value);
        string LegLabel(int o, int d) =>
            $"{(iata.TryGetValue(o, out var oi) ? oi : $"#{o}")}→{(iata.TryGetValue(d, out var di) ? di : $"#{d}")}";

        var active = routes.Where(r => r.Active).OrderBy(r => r.Id.Value).ToList();
        if (!active.Any())
            throw new InvalidOperationException("No hay rutas activas. Crea/activa rutas primero.");

        var pickedOut = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecciona la ruta de IDA (origen→destino):")
                .PageSize(12)
                .AddChoices(active.Select(r => $"{r.Id.Value}. {LegLabel(r.OriginAirport, r.DestinationAirport)}")));
        var idRouteOut = int.Parse(pickedOut.Split(new char[] { '.' })[0]);
        var routeOut = active.First(r => r.Id.Value == idRouteOut);

        var routeRet = active.FirstOrDefault(r =>
            r.OriginAirport == routeOut.DestinationAirport && r.DestinationAirport == routeOut.OriginAirport);
        if (routeRet is null)
        {
            AnsiConsole.MarkupLine(
                $"\n[red]No existe ruta activa de regreso[/] ({Markup.Escape(LegLabel(routeOut.DestinationAirport, routeOut.OriginAirport))}).");
            if (!AnsiConsole.Confirm("¿Deseas crear/activar ahora la ruta de regreso (destino→origen) para poder publicar ida y vuelta?", true))
            {
                AnsiConsole.MarkupLine("[grey]Operación cancelada. Presiona cualquier tecla para continuar...[/]");
                Console.ReadKey();
                return;
            }

            // Crear (o asegurar) la ruta inversa en este mismo flujo
            var idRouteRet = await EnsureActiveRouteAsync(routeOut.DestinationAirport, routeOut.OriginAirport, ct);
            // Refrescar lista de rutas activas y obtener el agregado de regreso para usarlo más abajo
            using var refreshCtx = DbContextFactory.Create();
            var refreshed = await new GetAllRoutesUseCase(new RouteRepository(refreshCtx)).ExecuteAsync(ct);
            routeRet = refreshed.First(r => r.Id.Value == idRouteRet);
        }

        var dateOutStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Fecha de IDA (yyyy-MM-dd):")
                .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido[/]")));
        var dateOutbound = DateOnly.ParseExact(dateOutStr, "yyyy-MM-dd");

        var dateRetStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Fecha de VUELTA / REGRESO (yyyy-MM-dd):")
                .Validate(s =>
                {
                    if (!DateOnly.TryParseExact(s, "yyyy-MM-dd", out var d))
                        return ValidationResult.Error("[red]Formato inválido[/]");
                    if (d < dateOutbound)
                        return ValidationResult.Error("[red]La vuelta no puede ser anterior a la ida.[/]");
                    return ValidationResult.Success();
                }));
        var dateReturn = DateOnly.ParseExact(dateRetStr, "yyyy-MM-dd");

        var numOut = AnsiConsole.Ask<string>("Número de vuelo de IDA (ej: AV101):").Trim();
        var numRet = AnsiConsole.Ask<string>("Número de vuelo de VUELTA (ej: AV102):").Trim();

        var depOutStr = AnsiConsole.Prompt(
            new TextPrompt<string>("IDA — Hora de salida (HH:mm):")
                .Validate(s => TimeOnly.TryParseExact(s, "HH:mm", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido[/]")));
        var arrOutStr = AnsiConsole.Prompt(
            new TextPrompt<string>("IDA — Hora de llegada (HH:mm):")
                .Validate(s => TimeOnly.TryParseExact(s, "HH:mm", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido[/]")));
        var depOut = TimeOnly.ParseExact(depOutStr, "HH:mm");
        var arrOut = TimeOnly.ParseExact(arrOutStr, "HH:mm");
        if (arrOut <= depOut)
            throw new InvalidOperationException("En la ida la llegada debe ser posterior a la salida.");

        var depRetStr = AnsiConsole.Prompt(
            new TextPrompt<string>("VUELTA — Hora de salida (HH:mm):")
                .Validate(s => TimeOnly.TryParseExact(s, "HH:mm", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido[/]")));
        var arrRetStr = AnsiConsole.Prompt(
            new TextPrompt<string>("VUELTA — Hora de llegada (HH:mm):")
                .Validate(s => TimeOnly.TryParseExact(s, "HH:mm", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido[/]")));
        var depRet = TimeOnly.ParseExact(depRetStr, "HH:mm");
        var arrRet = TimeOnly.ParseExact(arrRetStr, "HH:mm");
        if (arrRet <= depRet)
            throw new InvalidOperationException("En la vuelta la llegada debe ser posterior a la salida.");

        try
        {
            var (idAircraft, capacity, airlineId) = await SelectAircraftAsync(ct);
            var seatsToSell = AnsiConsole.Prompt(
                new TextPrompt<int>($"Asientos disponibles para vender (máx {capacity}; se aplicará a ida y vuelta):")
                    .Validate(v =>
                        v > 0 && v <= capacity
                            ? ValidationResult.Success()
                            : ValidationResult.Error($"[red]Debe estar entre 1 y {capacity}[/]")));

            var idStatus = await SelectStatusAsync("Estado inicial de ambos vuelos:", ct);
            var idCrew = await SelectCrewAsync(ct);
            var gateOut = (AnsiConsole.Prompt(
                    new TextPrompt<string>("IDA — Puerta de embarque (ej. A12):")
                        .DefaultValue("A12")
                        .AllowEmpty())
                ?? "A12").Trim();
            if (string.IsNullOrEmpty(gateOut)) gateOut = "A12";
            var gateRet = (AnsiConsole.Prompt(
                    new TextPrompt<string>("VUELTA — Puerta de embarque (Enter = misma que ida):")
                        .DefaultValue(gateOut)
                        .AllowEmpty())
                ?? gateOut).Trim();
            if (string.IsNullOrEmpty(gateRet)) gateRet = gateOut;

            // Tarifa (por clase) para cada fecha
            var idFareOut = await EnsureFareForAirlineAndDateAsync(airlineId, idAircraft, dateOutbound, ct);
            var idFareRet = await EnsureFareForAirlineAndDateAsync(airlineId, idAircraft, dateReturn, ct);

            using var context = DbContextFactory.Create();
            var flightRepo = new FlightRepository(context);
            await new CreateFlightUseCase(flightRepo)
                .ExecuteAsync(numOut, dateOutbound, depOut, arrOut, capacity, seatsToSell, idRouteOut, idAircraft, idStatus, idCrew, idFareOut, gateOut, ct);
            await context.SaveChangesAsync(ct);

            await new CreateFlightUseCase(flightRepo)
                .ExecuteAsync(numRet, dateReturn, depRet, arrRet, capacity, seatsToSell, routeRet.Id.Value, idAircraft, idStatus, idCrew, idFareRet, gateRet, ct);
            await context.SaveChangesAsync(ct);

            var all = await new GetAllFlightsUseCase(flightRepo).ExecuteAsync(ct);
            var idOut = all.Where(f => f.Number.Value == numOut && f.Date.Value == dateOutbound && f.IdRoute == idRouteOut)
                .OrderByDescending(f => f.Id.Value).Select(f => f.Id.Value).FirstOrDefault();
            var idRet = all.Where(f => f.Number.Value == numRet && f.Date.Value == dateReturn && f.IdRoute == routeRet.Id.Value)
                .OrderByDescending(f => f.Id.Value).Select(f => f.Id.Value).FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]IDA {Markup.Escape(LegLabel(routeOut.OriginAirport, routeOut.DestinationAirport))} — vuelo {Markup.Escape(numOut)} ID {idOut}.[/]");
            AnsiConsole.MarkupLine($"[green]VUELTA {Markup.Escape(LegLabel(routeRet.OriginAirport, routeRet.DestinationAirport))} — vuelo {Markup.Escape(numRet)} ID {idRet}.[/]");

            if (idOut > 0 && AnsiConsole.Confirm("\n¿Generar asientos del vuelo de IDA ahora?", true))
                await GenerateSeatFlightsAutoAsync(idOut, idAircraft, capacity, ct);
            if (idRet > 0 && AnsiConsole.Confirm("\n¿Generar asientos del vuelo de VUELTA ahora?", true))
                await GenerateSeatFlightsAutoAsync(idRet, idAircraft, capacity, ct);
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task CreateGuidedAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR VUELO — FLUJO GUIADO[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un vuelo?", true))
            return;
        // Para entender el cupo máximo, primero seleccionamos aerolínea/aeronave (capacidad).
        int idAircraft;
        int capacity;
        int airlineId;
        try
        {
            (idAircraft, capacity, airlineId) = await SelectAircraftAsync(ct);
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
            return;
        }

        var number = AnsiConsole.Ask<string>("Número de vuelo (ej: AV123):");
        var dateStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Fecha del vuelo (yyyy-MM-dd):")
                .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                    ? ValidationResult.Success() : ValidationResult.Error("[red]Formato inválido[/]")));
        var depStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Hora de salida (HH:mm):")
                .Validate(s => TimeOnly.TryParseExact(s, "HH:mm", out _)
                    ? ValidationResult.Success() : ValidationResult.Error("[red]Formato inválido[/]")));
        var arrStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Hora de llegada (HH:mm):")
                .Validate(s => TimeOnly.TryParseExact(s, "HH:mm", out _)
                    ? ValidationResult.Success() : ValidationResult.Error("[red]Formato inválido[/]")));
        var date = DateOnly.ParseExact(dateStr, "yyyy-MM-dd");
        var dep = TimeOnly.ParseExact(depStr, "HH:mm");
        var arr = TimeOnly.ParseExact(arrStr, "HH:mm");
        if (date < DateOnly.FromDateTime(DateTime.Today))
        {
            AnsiConsole.MarkupLine("\n[red]La fecha del vuelo no puede ser anterior a hoy.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false); return;
        }
        if (arr <= dep)
        {
            AnsiConsole.MarkupLine("\n[red]La hora de llegada debe ser posterior a la hora de salida.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false); return;
        }
        try
        {
            var idRoute = await SelectRouteAsync(ct);
            var idStatus = await SelectStatusAsync("Estado inicial del vuelo:", ct);
            var idCrew = await SelectCrewAsync(ct);
            var availableSeats = AnsiConsole.Prompt(
                new TextPrompt<int>(
                        $"Asientos disponibles para vender (máx {capacity}; Enter = vender todos):")
                    .DefaultValue(capacity)
                    .Validate(v =>
                        v > 0 && v <= capacity
                            ? ValidationResult.Success()
                            : ValidationResult.Error($"[red]Debe estar entre 1 y {capacity}[/]")));

            // Integración solicitada: tarifa dentro del flujo de publicación del vuelo.
            var idFare = await EnsureFareForAirlineAndDateAsync(airlineId, idAircraft, date, ct);
            var gate = (AnsiConsole.Prompt(
                    new TextPrompt<string>("Puerta de embarque (ej. A12):")
                        .DefaultValue("A12")
                        .AllowEmpty())
                ?? "A12").Trim();
            if (string.IsNullOrEmpty(gate)) gate = "A12";

            using var context = DbContextFactory.Create();
            var flight = await new CreateFlightUseCase(new FlightRepository(context))
                .ExecuteAsync(number, date, dep, arr, capacity, availableSeats, idRoute, idAircraft, idStatus, idCrew, idFare, gate, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllFlightsUseCase(new FlightRepository(context)).ExecuteAsync(ct))
                .Where(f =>
                    f.Number.Value == number &&
                    f.Date.Value == date &&
                    f.DepartureTime.Value == dep &&
                    f.ArrivalTime.Value == arr &&
                    f.IdRoute == idRoute &&
                    f.IdAircraft == idAircraft &&
                    f.IdStatus == idStatus &&
                    f.IdCrew == idCrew)
                .OrderByDescending(f => f.Id.Value)
                .Select(f => f.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Vuelo [bold]{Markup.Escape(flight.Number.Value)}[/] creado con ID {createdId}.[/]");

            if (AnsiConsole.Confirm("\n¿Generar asientos del vuelo automáticamente ahora? (SeatFlight)", true))
                await GenerateSeatFlightsAutoAsync(createdId, idAircraft, capacity, ct);
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task GenerateSeatFlightsAutoAsync(int idFlight, int idAircraft, int aircraftCapacity, CancellationToken ct)
    {
        // En el sistema, “asientos automáticos” para un vuelo significa crear SeatFlight en base
        // a los asientos físicos (Seat) registrados para la aeronave.
        // Si el avión aún no tiene asientos, ofrecemos generarlos automáticamente aquí mismo.
        using var precheckContext = DbContextFactory.Create();
        var existingSeats = await new GetAllSeatsUseCase(new SeatRepository(precheckContext)).ExecuteAsync(ct);
        var aircraftSeats = existingSeats.Where(s => s.IdAircraft == idAircraft).ToList();

        if (!aircraftSeats.Any())
        {
            AnsiConsole.MarkupLine("[yellow]La aeronave no tiene asientos registrados.[/]");
            if (!AnsiConsole.Confirm("¿Deseas generarlos automáticamente ahora (según la capacidad del avión)?", true))
            {
                AnsiConsole.MarkupLine("[grey]No se generaron asientos. Puedes crearlos manualmente en «Aeronaves y asientos».[/]");
                return;
            }

            await GenerateAircraftSeatsAsync(idAircraft, aircraftCapacity, ct);
        }

        await GenerateSeatFlightsAsync(idFlight, idAircraft, ct);
    }

    private static async Task GenerateAircraftSeatsAsync(int idAircraft, int totalCapacity, CancellationToken ct)
    {
        // Reutiliza la misma lógica del menú de aeronaves: distribuir asientos por clase y generar numeración.
        using var classContext = DbContextFactory.Create();
        var seatClasses = await new GetAllSeatClassesUseCase(new SeatClassRepository(classContext)).ExecuteAsync(ct);
        if (!seatClasses.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay clases de asiento registradas. Crea clases primero (Administración → Clases de asiento).[/]");
            return;
        }

        AnsiConsole.MarkupLine($"\n[grey]Distribuye los {totalCapacity} asientos por clase:[/]");
        var distribution = new List<(int idClass, string className, int count)>();
        int remaining = totalCapacity;

        foreach (var cls in seatClasses)
        {
            if (remaining <= 0) break;
            var count = AnsiConsole.Ask<int>($"  Asientos de clase '[bold]{Markup.Escape(cls.Name.Value)}[/]' (quedan {remaining}):");
            count = Math.Min(count, remaining);
            if (count > 0) distribution.Add((cls.Id.Value, cls.Name.Value, count));
            remaining -= count;
        }

        if (!distribution.Any())
        {
            AnsiConsole.MarkupLine("[grey]No se generaron asientos porque la distribución fue 0.[/]");
            return;
        }

        using var context = DbContextFactory.Create();
        var seatRepo = new SeatRepository(context);
        var useCase = new CreateSeatUseCase(seatRepo);
        char[] columns = ['A', 'B', 'C', 'D', 'E', 'F'];
        int globalRow = 1;
        int colIndex = 0;

        await AnsiConsole.Progress().StartAsync(async progressCtx =>
        {
            foreach (var (idClass, className, count) in distribution)
            {
                var task = progressCtx.AddTask($"Generando asientos {className}", maxValue: count);
                for (int i = 0; i < count; i++)
                {
                    var seatNumber = $"{globalRow}{columns[colIndex]}";
                    await useCase.ExecuteAsync(seatNumber, idAircraft, idClass, ct);
                    colIndex++;
                    if (colIndex >= columns.Length) { colIndex = 0; globalRow++; }
                    task.Increment(1);
                }
            }
        });

        await context.SaveChangesAsync(ct);
        AnsiConsole.MarkupLine($"\n[green]Se generaron {distribution.Sum(d => d.count)} asientos en la aeronave.[/]");
    }

    private static async Task GenerateSeatFlightsAsync(int idFlight, int idAircraft, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var seats = await new GetAllSeatsUseCase(new SeatRepository(context)).ExecuteAsync(ct);
        var aircraftSeats = seats.Where(s => s.IdAircraft == idAircraft).ToList();
        if (!aircraftSeats.Any())
        {
            AnsiConsole.MarkupLine("[yellow]La aeronave no tiene asientos registrados. Agrégalos (o genéralos) primero desde Aeronaves.[/]");
            return;
        }

        // Si ya existen asientos generados para este vuelo, no repetir.
        var existingCount = await context.Set<SeatFlightEntity>()
            .AsNoTracking()
            .CountAsync(sf => sf.IdFlight == idFlight, ct);
        if (existingCount > 0)
        {
            AnsiConsole.MarkupLine(
                $"[grey]Este vuelo ya tiene asientos generados (SeatFlight):[/] [bold]{existingCount}[/]. No se generarán de nuevo.");
            return;
        }

        var useCase = new CreateSeatFlightUseCase(new SeatFlightRepository(context));
        await AnsiConsole.Progress().StartAsync(async progressCtx =>
        {
            var task = progressCtx.AddTask("Generando asientos del vuelo", maxValue: aircraftSeats.Count);
            foreach (var seat in aircraftSeats)
            {
                await useCase.ExecuteAsync(seat.Id.Value, idFlight, true, ct);
                task.Increment(1);
            }
        });
        await context.SaveChangesAsync(ct);
        AnsiConsole.MarkupLine($"\n[green]Se generaron {aircraftSeats.Count} asientos para el vuelo.[/]");
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR VUELO[/]").Centered());
        AnsiConsole.MarkupLine(
            "[grey]Recordatorio de formatos: número de vuelo [bold]XX123[/] (2 letras de aerolínea + 1 a 4 dígitos, ej. [bold]AV908[/], [bold]LA45[/]); " +
            "fecha [bold]yyyy-MM-dd[/] (ej. 2026-08-13); horas [bold]HH:mm[/] en 24 h (ej. 10:20, 21:05).[/]");
        AnsiConsole.WriteLine();
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del vuelo a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var number = AnsiConsole.Prompt(
                new TextPrompt<string>(
                        "Nuevo número de vuelo (2 letras + 1-4 dígitos, ej. AV101, LA4567; se guarda en mayúsculas):")
                    .Validate(s =>
                    {
                        if (string.IsNullOrWhiteSpace(s))
                            return ValidationResult.Error("[red]Ingresá un número de vuelo (no puede estar vacío).[/]");
                        try
                        {
                            _ = FlightNumber.Create(s);
                            return ValidationResult.Success();
                        }
                        catch
                        {
                            return ValidationResult.Error(
                                "[red]Formato inválido: deben ser 2 letras (código IATA de aerolínea) y luego 1 a 4 dígitos, sin espacios. Ejemplos válidos: AV123, LA9, IB4567.[/]");
                        }
                    }))
            .Trim();
        number = FlightNumber.Create(number).Value;
        var dateStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Nueva fecha (yyyy-MM-dd, ej. 2026-08-13):")
                .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                    ? ValidationResult.Success() : ValidationResult.Error("[red]Usá exactamente yyyy-MM-dd, ej. 2026-08-13.[/]")));
        var depStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Nueva hora de salida (HH:mm en 24 h, ej. 10:20):")
                .Validate(s => TimeOnly.TryParseExact(s, "HH:mm", out _)
                    ? ValidationResult.Success() : ValidationResult.Error("[red]Usá HH:mm con dos dígitos para hora y minutos, ej. 09:05 o 14:30.[/]")));
        var arrStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Nueva hora de llegada (HH:mm en 24 h, ej. 11:30):")
                .Validate(s => TimeOnly.TryParseExact(s, "HH:mm", out _)
                    ? ValidationResult.Success() : ValidationResult.Error("[red]Usá HH:mm con dos dígitos para hora y minutos, ej. 09:05 o 14:30.[/]")));
        var date = DateOnly.ParseExact(dateStr, "yyyy-MM-dd");
        var dep = TimeOnly.ParseExact(depStr, "HH:mm");
        var arr = TimeOnly.ParseExact(arrStr, "HH:mm");
        if (date < DateOnly.FromDateTime(DateTime.Today))
        {
            AnsiConsole.MarkupLine("\n[red]La fecha del vuelo no puede ser anterior a hoy.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false); return;
        }
        if (arr <= dep)
        {
            AnsiConsole.MarkupLine("\n[red]La hora de llegada debe ser posterior a la hora de salida.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false); return;
        }
        try
        {
            var idRoute = await SelectRouteAsync(ct);
            var (idAircraft, capacity, _) = await SelectAircraftAsync(ct);
            var availableSeats = AnsiConsole.Prompt(
                new TextPrompt<int>(
                        $"Asientos disponibles para vender (máx {capacity}; Enter = vender todos):")
                    .DefaultValue(capacity)
                    .Validate(v =>
                        v > 0 && v <= capacity
                            ? ValidationResult.Success()
                            : ValidationResult.Error($"[red]Debe estar entre 1 y {capacity}[/]")));
            var idStatus = await SelectStatusAsync("Nuevo estado del vuelo:", ct);
            var idCrew = await SelectCrewAsync(ct);
            // Mantener la tarifa actual del vuelo (o recalcular si se cambia fecha/aerolínea sería una mejora futura)
            using var lookupCtx = DbContextFactory.Create();
            var current = await new GetFlightByIdUseCase(new FlightRepository(lookupCtx)).ExecuteAsync(id, ct);
            var idFare = current.IdFare;
            var gateU = (AnsiConsole.Prompt(
                    new TextPrompt<string>("Puerta de embarque (Enter = mantener actual):")
                        .DefaultValue(current.BoardingGate)
                        .AllowEmpty())
                ?? current.BoardingGate).Trim();
            if (string.IsNullOrEmpty(gateU)) gateU = current.BoardingGate;
            using var context = DbContextFactory.Create();
            await new UpdateFlightUseCase(new FlightRepository(context))
                .ExecuteAsync(id, number, date, dep, arr, capacity, availableSeats, idRoute, idAircraft, idStatus, idCrew, idFare, gateU, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Vuelo actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task AddStatusHistoryAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]REGISTRAR CAMBIO DE ESTADO[/]").Centered());
        var idFlight = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del vuelo (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (idFlight == 0) return;
        var observation = AnsiConsole.Prompt(
                new TextPrompt<string>("Observación (Enter para omitir):")
                    .AllowEmpty())
            .Trim();
        string? obs = string.IsNullOrEmpty(observation) ? null : observation;
        try
        {
            var idStatus = await SelectStatusAsync("Nuevo estado:", ct);
            using var context = DbContextFactory.Create();
            var flight = await new GetFlightByIdUseCase(new FlightRepository(context)).ExecuteAsync(idFlight, ct);
            await new CreateFlightStatusHistoryUseCase(new FlightStatusHistoryRepository(context))
                .ExecuteAsync(DateTime.Now, obs, idFlight, idStatus, AppState.IdUser, ct);
            await new UpdateFlightUseCase(new FlightRepository(context))
                .ExecuteAsync(
                    flight.Id.Value,
                    flight.Number.Value,
                    flight.Date.Value,
                    flight.DepartureTime.Value,
                    flight.ArrivalTime.Value,
                    flight.TotalCapacity.Value,
                    flight.AvailableSeats.Value,
                    flight.IdRoute,
                    flight.IdAircraft,
                    idStatus,
                    flight.IdCrew,
                    flight.IdFare,
                    flight.BoardingGate,
                    ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Estado del vuelo actualizado (historial + estado actual).[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR VUELO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del vuelo a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el vuelo con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteFlightUseCase(new FlightRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Vuelo eliminado correctamente.[/]" : "\n[yellow]No se encontró el vuelo con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
