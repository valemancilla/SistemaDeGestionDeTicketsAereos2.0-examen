using SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;
using Microsoft.EntityFrameworkCore;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.UI;

public sealed class AirportMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE AEROPUERTOS[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(8)
                    .AddChoices("1. Crear aeropuerto", "2. Listar aeropuertos", "3. Actualizar aeropuerto",
                                "4. Asignar zona horaria", "5. Activar / Desactivar", "6. Eliminar aeropuerto", "0. Volver"));

            switch (option)
            {
                case "1. Crear aeropuerto": await CreateAsync(ct); break;
                case "2. Listar aeropuertos": await ListAsync(ct); break;
                case "3. Actualizar aeropuerto": await UpdateAsync(ct); break;
                case "4. Asignar zona horaria": await AssignTimeZoneAsync(ct); break;
                case "5. Activar / Desactivar": await ToggleActiveAsync(ct); break;
                case "6. Eliminar aeropuerto": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var airports = await new GetAllAirportsUseCase(new AirportRepository(context)).ExecuteAsync(ct);
        var cities = await new GetAllCitiesUseCase(new CityRepository(context)).ExecuteAsync(ct);
        var countries = await new GetAllCountriesUseCase(new CountryRepository(context)).ExecuteAsync(ct);
        var timeZones = await new GetAllTimeZonesUseCase(new TimeZoneRepository(context)).ExecuteAsync(ct);

        var cityMap = cities.ToDictionary(c => c.Id.Value, c => (name: c.Name.Value, idCountry: c.IdCountry));
        var countryMap = countries.ToDictionary(c => c.Id.Value, c => c.Name.Value);
        var tzMap = timeZones.ToDictionary(z => z.Id.Value, z => $"{z.Name.Value} ({z.UTCOffset.Value})");

        var airportTzLinks = await context.Set<AirportTimeZoneEntity>()
            .AsNoTracking()
            .ToListAsync(ct);
        var airportTzMap = airportTzLinks
            .GroupBy(x => x.IdAirport)
            .ToDictionary(
                g => g.Key,
                g => string.Join(", ",
                    g.Select(x => tzMap.TryGetValue(x.IdTimeZone, out var tz) ? tz : x.IdTimeZone.ToString())
                        .Distinct()
                        .OrderBy(x => x)));

        if (!airports.Any()) { AnsiConsole.MarkupLine("[yellow]No hay aeropuertos registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID");
            table.AddColumn("Nombre");
            table.AddColumn("IATA");
            table.AddColumn("Ciudad");
            table.AddColumn("País");
            table.AddColumn("Zona horaria (UTC)");
            table.AddColumn("Activo");
            foreach (var a in airports)
            {
                var city = cityMap.TryGetValue(a.IdCity, out var cn) ? cn.name : a.IdCity.ToString();
                var idCountry = cityMap.TryGetValue(a.IdCity, out var cdata) ? cdata.idCountry : 0;
                var country = (idCountry > 0 && countryMap.TryGetValue(idCountry, out var ctry)) ? ctry : (idCountry > 0 ? idCountry.ToString() : "-");
                var tz = airportTzMap.TryGetValue(a.Id.Value, out var tzStr) && !string.IsNullOrWhiteSpace(tzStr)
                    ? tzStr
                    : "-";
                table.AddRow(a.Id.Value.ToString(), Markup.Escape(a.Name.Value), Markup.Escape(a.IATACode.Value),
                    Markup.Escape(city),
                    Markup.Escape(country),
                    Markup.Escape(tz),
                    a.Active ? "[green]Sí[/]" : "[red]No[/]");
            }
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task<int> SelectCityAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var cities = await new GetAllCitiesUseCase(new CityRepository(context)).ExecuteAsync(ct);
        if (!cities.Any()) throw new InvalidOperationException("No hay ciudades registradas. Crea una primero.");
        cities = cities.OrderBy(c => c.Id.Value).ToList();
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona la ciudad:").PageSize(10)
                .AddChoices(cities.Select(c => $"{c.Id.Value}. {c.Name.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR AEROPUERTO[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un aeropuerto?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre del aeropuerto:");
        var iata = AnsiConsole.Ask<string>("Código IATA (3 letras, ej: BOG, MIA):");
        try
        {
            var idCity = await SelectCityAsync(ct);
            var active = AnsiConsole.Confirm("¿Aeropuerto activo?", true);
            using var context = DbContextFactory.Create();
            var result = await new CreateAirportUseCase(new AirportRepository(context)).ExecuteAsync(name, iata, idCity, active, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllAirportsUseCase(new AirportRepository(context)).ExecuteAsync(ct))
                .Where(a => a.Name.Value == name && a.IATACode.Value == iata && a.IdCity == idCity && a.Active == active)
                .OrderByDescending(a => a.Id.Value)
                .Select(a => a.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Aeropuerto '[bold]{Markup.Escape(result.Name.Value)}[/]' ({result.IATACode.Value}) creado con ID {createdId}.[/]");

            if (AnsiConsole.Confirm("\n¿Asignar zona horaria ahora?", true))
                await AssignTimeZoneToAirportAsync(createdId, ct);
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR AEROPUERTO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del aeropuerto a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        var iata = AnsiConsole.Ask<string>("Nuevo código IATA:");
        try
        {
            var idCity = await SelectCityAsync(ct);
            var active = AnsiConsole.Confirm("¿Aeropuerto activo?", true);
            using var context = DbContextFactory.Create();
            await new UpdateAirportUseCase(new AirportRepository(context)).ExecuteAsync(id, name, iata, idCity, active, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Aeropuerto actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task ToggleActiveAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTIVAR / DESACTIVAR AEROPUERTO[/]").Centered());
        using var listContext = DbContextFactory.Create();
        var airports = await new GetAllAirportsUseCase(new AirportRepository(listContext)).ExecuteAsync(ct);
        if (!airports.Any()) { AnsiConsole.MarkupLine("[yellow]No hay aeropuertos registrados.[/]"); Console.ReadKey(); return; }

        var selected = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Selecciona el aeropuerto:").PageSize(10)
            .AddChoices(airports.Select(a => $"{a.Id.Value}. {a.Name.Value} ({a.IATACode.Value}) — {(a.Active ? "Activo" : "Inactivo")}")));
        var selectedId = int.Parse(selected.Split(new char[] { '.' })[0]);
        var airport = airports.First(a => a.Id.Value == selectedId);

        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateAirportUseCase(new AirportRepository(context)).ExecuteAsync(
                airport.Id.Value, airport.Name.Value, airport.IATACode.Value, airport.IdCity, !airport.Active, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine($"\n[green]Aeropuerto '{Markup.Escape(airport.Name.Value)}' ahora está {(!airport.Active ? "ACTIVO" : "INACTIVO")}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task AssignTimeZoneAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ASIGNAR ZONA HORARIA A AEROPUERTO[/]").Centered());
        using var listContext = DbContextFactory.Create();
        var airports = await new GetAllAirportsUseCase(new AirportRepository(listContext)).ExecuteAsync(ct);
        if (!airports.Any()) { AnsiConsole.MarkupLine("[yellow]No hay aeropuertos registrados.[/]"); Console.ReadKey(); return; }

        var selected = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Selecciona el aeropuerto:").PageSize(10)
            .AddChoices(airports.Select(a => $"{a.Id.Value}. {a.Name.Value} ({a.IATACode.Value})")));
        var idAirport = int.Parse(selected.Split(new char[] { '.' })[0]);

        await AssignTimeZoneToAirportAsync(idAirport, ct);
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task AssignTimeZoneToAirportAsync(int idAirport, CancellationToken ct)
    {
        using var tzContext = DbContextFactory.Create();
        var timeZones = await new GetAllTimeZonesUseCase(new TimeZoneRepository(tzContext)).ExecuteAsync(ct);
        if (!timeZones.Any()) { AnsiConsole.MarkupLine("[yellow]No hay zonas horarias registradas. Crea una en Administración.[/]"); return; }

        var selectedTz = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Selecciona la zona horaria:").PageSize(10)
            .AddChoices(timeZones.Select(z => $"{z.Id.Value}. {z.Name.Value} ({z.UTCOffset.Value})")));
        var idTimeZone = int.Parse(selectedTz.Split(new char[] { '.' })[0]);

        try
        {
            using var context = DbContextFactory.Create();
            await new CreateAirportTimeZoneUseCase(new AirportTimeZoneRepository(context)).ExecuteAsync(idAirport, idTimeZone, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Zona horaria asignada correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR AEROPUERTO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del aeropuerto a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;

        try
        {
            using var precheckContext = DbContextFactory.Create();

            var airport = await new GetAirportByIdUseCase(new AirportRepository(precheckContext)).ExecuteAsync(id, ct);
            if (airport is null)
            {
                AnsiConsole.MarkupLine("\n[yellow]No se encontró el aeropuerto con ese ID.[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
                return;
            }

            // Dependencias que bloquean el delete por FK (Restrict)
            var routesCount = await precheckContext.Set<RouteEntity>()
                .CountAsync(r => r.OriginAirport == id || r.DestinationAirport == id, ct);

            // Tabla puente: se puede limpiar automáticamente antes del delete
            var tzLinksCount = await precheckContext.Set<AirportTimeZoneEntity>()
                .CountAsync(x => x.IdAirport == id, ct);

            AnsiConsole.MarkupLine($"\nAeropuerto: [bold]{Markup.Escape(airport.Name.Value)}[/] ({airport.IATACode.Value})");
            if (routesCount > 0)
            {
                AnsiConsole.MarkupLine($"\n[red]No se puede eliminar[/] porque está asociado a [bold]{routesCount}[/] ruta(s).");
                AnsiConsole.MarkupLine("[grey]Primero elimina o reasigna esas rutas (y sus vuelos), luego intenta de nuevo.[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
                return;
            }

            if (!AnsiConsole.Confirm($"¿Confirma eliminar el aeropuerto con ID {id}?"))
            { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }

            if (tzLinksCount > 0)
            {
                AnsiConsole.MarkupLine($"[yellow]Nota:[/] Se eliminarán también {tzLinksCount} vínculo(s) de zona horaria del aeropuerto.");
            }

            using var context = DbContextFactory.Create();
            var deleted = await new DeleteAirportUseCase(new AirportRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted
                ? "\n[green]Aeropuerto eliminado correctamente.[/]"
                : "\n[yellow]No se encontró el aeropuerto con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
