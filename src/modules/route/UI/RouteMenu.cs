using SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.UI;

public sealed class RouteMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE RUTAS[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(7)
                    .AddChoices("1. Crear ruta", "2. Listar rutas", "3. Actualizar ruta", "4. Activar / Desactivar", "5. Eliminar ruta", "0. Volver"));

            switch (option)
            {
                case "1. Crear ruta":          await CreateAsync(ct);       break;
                case "2. Listar rutas":       await ListAsync(ct);         break;
                case "3. Actualizar ruta":     await UpdateAsync(ct);       break;
                case "4. Activar / Desactivar":await ToggleActiveAsync(ct); break;
                case "5. Eliminar ruta":       await DeleteAsync(ct);       break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var routes   = await new GetAllRoutesUseCase(new RouteRepository(context)).ExecuteAsync(ct);
        var airports = await new GetAllAirportsUseCase(new AirportRepository(context)).ExecuteAsync(ct);
        var airportMap = airports.ToDictionary(a => a.Id.Value, a => $"{a.Name.Value} ({a.IATACode.Value})");

        if (!routes.Any()) { AnsiConsole.MarkupLine("[yellow]No hay rutas registradas.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Origen"); table.AddColumn("Destino");
            table.AddColumn("Distancia (km)"); table.AddColumn("Duración"); table.AddColumn("Activa");
            foreach (var r in routes)
            {
                var origin = airportMap.TryGetValue(r.OriginAirport, out var on) ? on : r.OriginAirport.ToString();
                var dest   = airportMap.TryGetValue(r.DestinationAirport, out var dn) ? dn : r.DestinationAirport.ToString();
                table.AddRow(r.Id.Value.ToString(), Markup.Escape(origin), Markup.Escape(dest),
                    r.DistanceKm.Value.ToString("F1"), r.EstDuration.Value.ToString("HH:mm"),
                    r.Active ? "[green]Sí[/]" : "[red]No[/]");
            }
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task<int> SelectAirportAsync(string prompt, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var airports = await new GetAllAirportsUseCase(new AirportRepository(context)).ExecuteAsync(ct);
        if (!airports.Any()) throw new InvalidOperationException("No hay aeropuertos registrados. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title(prompt).PageSize(10)
                .AddChoices(airports.Select(a => $"{a.Id.Value}. {a.Name.Value} ({a.IATACode.Value})")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR RUTA[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear una ruta?", true))
            return;
        var distance = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Distancia en km (ej: 850.5):")
                .Validate(v => v > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]La distancia debe ser mayor a 0[/]")));
        var durationStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Duración estimada (HH:MM, ej: 01:45):")
                .Validate(s => TimeOnly.TryParseExact(s, "HH:mm", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido. Use HH:MM (ej: 01:45)[/]")));
        var duration = TimeOnly.ParseExact(durationStr, "HH:mm");
        try
        {
            var idOrigin = await SelectAirportAsync("Selecciona aeropuerto de ORIGEN:", ct);
            var idDest   = await SelectAirportAsync("Selecciona aeropuerto de DESTINO:", ct);
            if (idOrigin == idDest) throw new InvalidOperationException("El aeropuerto de origen y destino no pueden ser el mismo.");
            var active   = AnsiConsole.Confirm("¿Ruta activa?", true);
            using var context = DbContextFactory.Create();
            var result = await new CreateRouteUseCase(new RouteRepository(context)).ExecuteAsync(distance, duration, idOrigin, idDest, active, ct);
            await context.SaveChangesAsync(ct);
            var createdId = await context.Set<RouteEntity>()
                .AsNoTracking()
                .Where(r =>
                    r.OriginAirport == idOrigin &&
                    r.DestinationAirport == idDest &&
                    r.DistanceKm == result.DistanceKm.Value &&
                    r.EstDuration == result.EstDuration.Value &&
                    r.Active == active)
                .OrderByDescending(r => r.IdRoute)
                .Select(r => r.IdRoute)
                .FirstAsync(ct);
            AnsiConsole.MarkupLine($"\n[green]Ruta creada con ID {createdId} ({result.DistanceKm.Value:F1} km, {result.EstDuration.Value:HH:mm}h).[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR RUTA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la ruta a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var distance = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Nueva distancia en km:")
                .Validate(v => v > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]La distancia debe ser mayor a 0[/]")));
        var durationStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Nueva duración (HH:MM):")
                .Validate(s => TimeOnly.TryParseExact(s, "HH:mm", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido. Use HH:MM[/]")));
        var duration = TimeOnly.ParseExact(durationStr, "HH:mm");
        try
        {
            var idOrigin = await SelectAirportAsync("Selecciona aeropuerto de ORIGEN:", ct);
            var idDest   = await SelectAirportAsync("Selecciona aeropuerto de DESTINO:", ct);
            if (idOrigin == idDest) throw new InvalidOperationException("El aeropuerto de origen y destino no pueden ser el mismo.");
            var active   = AnsiConsole.Confirm("¿Ruta activa?", true);
            using var context = DbContextFactory.Create();
            await new UpdateRouteUseCase(new RouteRepository(context)).ExecuteAsync(id, distance, duration, idOrigin, idDest, active, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Ruta actualizada correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task ToggleActiveAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTIVAR / DESACTIVAR RUTA[/]").Centered());
        using var listContext = DbContextFactory.Create();
        var routes   = await new GetAllRoutesUseCase(new RouteRepository(listContext)).ExecuteAsync(ct);
        var airports = await new GetAllAirportsUseCase(new AirportRepository(listContext)).ExecuteAsync(ct);
        var airportMap = airports.ToDictionary(a => a.Id.Value, a => a.IATACode.Value);

        if (!routes.Any()) { AnsiConsole.MarkupLine("[yellow]No hay rutas registradas.[/]"); Console.ReadKey(); return; }

        var selected = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Selecciona la ruta:").PageSize(10)
            .AddChoices(routes.Select(r =>
            {
                var o = airportMap.TryGetValue(r.OriginAirport, out var ov) ? ov : r.OriginAirport.ToString();
                var d = airportMap.TryGetValue(r.DestinationAirport, out var dv) ? dv : r.DestinationAirport.ToString();
                return $"{r.Id.Value}. {o} → {d} — {(r.Active ? "Activa" : "Inactiva")}";
            })));
        var selectedId = int.Parse(selected.Split(new char[] { '.' })[0]);
        var route = routes.First(r => r.Id.Value == selectedId);

        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateRouteUseCase(new RouteRepository(context)).ExecuteAsync(
                route.Id.Value, route.DistanceKm.Value, route.EstDuration.Value,
                route.OriginAirport, route.DestinationAirport, !route.Active, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine($"\n[green]Ruta ahora está {(!route.Active ? "ACTIVA" : "INACTIVA")}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR RUTA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la ruta a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar la ruta con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteRouteUseCase(new RouteRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Ruta eliminada correctamente.[/]" : "\n[yellow]No se encontró la ruta con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }
}
