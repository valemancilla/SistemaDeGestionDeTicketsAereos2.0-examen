using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.UI;

public sealed class AircraftMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE AERONAVES[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(10)
                    .AddChoices("1. Registrar aeronave", "2. Listar aeronaves", "3. Ver asientos de aeronave",
                                "4. Listar todos los asientos", "5. Actualizar aeronave", "6. Eliminar aeronave", "0. Volver"));

            switch (option)
            {
                case "1. Registrar aeronave": await CreateAsync(ct); break;
                case "2. Listar aeronaves": await ListAsync(ct); break;
                case "3. Ver asientos de aeronave": await ListSeatsAsync(ct); break;
                case "4. Listar todos los asientos": await ListAllSeatsAsync(ct); break;
                case "5. Actualizar aeronave": await UpdateAsync(ct); break;
                case "6. Eliminar aeronave": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var aircrafts = await new GetAllAircraftsUseCase(new AircraftRepository(context)).ExecuteAsync(ct);
        var airlines = await new GetAllAerolinesUseCase(new AerolineRepository(context)).ExecuteAsync(ct);
        var models = await new GetAllAircraftModelsUseCase(new AircraftModelRepository(context)).ExecuteAsync(ct);
        var airlineMap = airlines.ToDictionary(a => a.Id.Value, a => a.Name.Value);
        var modelMap = models.ToDictionary(m => m.Id.Value, m => m.Name.Value);

        if (!aircrafts.Any()) { AnsiConsole.MarkupLine("[yellow]No hay aeronaves registradas.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Capacidad"); table.AddColumn("Aerolínea"); table.AddColumn("Modelo");
            foreach (var a in aircrafts)
            {
                var airline = airlineMap.TryGetValue(a.IdAirline, out var an) ? an : a.IdAirline.ToString();
                var model = modelMap.TryGetValue(a.IdModel, out var mn) ? mn : a.IdModel.ToString();
                table.AddRow(a.Id.Value.ToString(), a.Capacity.Value.ToString(), Markup.Escape(airline), Markup.Escape(model));
            }
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task ListAllSeatsAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]TODOS LOS ASIENTOS (CATÁLOGO)[/]").Centered());
        using var context = DbContextFactory.Create();
        var seats = await new GetAllSeatsUseCase(new SeatRepository(context)).ExecuteAsync(ct);
        var seatClasses = await new GetAllSeatClassesUseCase(new SeatClassRepository(context)).ExecuteAsync(ct);
        var classMap = seatClasses.ToDictionary(c => c.Id.Value, c => c.Name.Value);

        if (!seats.Any())
            AnsiConsole.MarkupLine("[yellow]No hay ningún asiento en la base de datos.[/]");
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("Id asiento");
            table.AddColumn("Id aeronave");
            table.AddColumn("Número");
            table.AddColumn("Clase");
            foreach (var s in seats.OrderBy(x => x.IdAircraft).ThenBy(x => x.Number.Value))
            {
                var cls = classMap.TryGetValue(s.IdClase, out var cn) ? cn : s.IdClase.ToString();
                table.AddRow(
                    s.Id.Value.ToString(),
                    s.IdAircraft.ToString(),
                    Markup.Escape(s.Number.Value),
                    Markup.Escape(cls));
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine(
                $"\n[grey]Total: {seats.Count}. La columna «Id aeronave» indica de qué avión es cada fila; " +
                "en «Ver asientos de aeronave» debes elegir ese mismo ID para verlos agrupados.[/]");
        }

        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task<int?> SelectAircraftIdFromDbAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var aircrafts = await new GetAllAircraftsUseCase(new AircraftRepository(context)).ExecuteAsync(ct);
        if (!aircrafts.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay aeronaves registradas.[/]");
            ConsolaPausa.PresionarCualquierTecla();
            return null;
        }

        var airlines = await new GetAllAerolinesUseCase(new AerolineRepository(context)).ExecuteAsync(ct);
        var models = await new GetAllAircraftModelsUseCase(new AircraftModelRepository(context)).ExecuteAsync(ct);
        var airlineMap = airlines.ToDictionary(a => a.Id.Value, a => a.Name.Value);
        var modelMap = models.ToDictionary(m => m.Id.Value, m => m.Name.Value);

        var choices = new List<string> { "0. Volver" };
        choices.AddRange(aircrafts.OrderBy(a => a.Id.Value).Select(a =>
        {
            var airline = airlineMap.TryGetValue(a.IdAirline, out var an) ? an : a.IdAirline.ToString();
            var model = modelMap.TryGetValue(a.IdModel, out var mn) ? mn : a.IdModel.ToString();
            return $"{a.Id.Value}. Cap. {a.Capacity.Value} — {airline} — {model}";
        }));

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecciona la aeronave para ver sus asientos:")
                .PageSize(15)
                .AddChoices(choices));

        if (selected.StartsWith("0.", StringComparison.Ordinal))
            return null;
        return int.Parse(selected.Split('.')[0], System.Globalization.CultureInfo.InvariantCulture);
    }

    private static async Task ListSeatsAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]ASIENTOS POR AERONAVE[/]").Centered());
        var idAircraft = await SelectAircraftIdFromDbAsync(ct);
        if (idAircraft is null) return;
        using var context = DbContextFactory.Create();
        var seatRepo = new SeatRepository(context);
        var seatClasses = await new GetAllSeatClassesUseCase(new SeatClassRepository(context)).ExecuteAsync(ct);
        var classMap = seatClasses.ToDictionary(c => c.Id.Value, c => c.Name.Value);
        var filtered = (await seatRepo.ListByAircraftAsync(idAircraft.Value, ct)).OrderBy(s => s.Number.Value).ToList();

        if (!filtered.Any())
        {
            AnsiConsole.MarkupLine($"[yellow]No hay asientos registrados en la aeronave con ID {idAircraft}.[/]");
            AnsiConsole.MarkupLine(
                "[grey]La capacidad del avión no crea asientos por sí sola: en la base de datos los asientos físicos (1A, 2B, etc.) " +
                "solo existen si se generaron al registrar la aeronave o si los generas ahora. " +
                "Los asientos asociados a un vuelo (reservas) usan estos asientos de aeronave.[/]");

            if (AnsiConsole.Confirm("\n¿Generar asientos ahora (misma lógica que al registrar una aeronave)?", false))
            {
                var aircraft = await new GetAircraftByIdUseCase(new AircraftRepository(context)).ExecuteAsync(idAircraft.Value, ct);
                await GenerateSeatsAsync(aircraft.Id.Value, aircraft.Capacity.Value, ct);
                filtered = (await seatRepo.ListByAircraftAsync(idAircraft.Value, ct)).OrderBy(s => s.Number.Value).ToList();
            }
        }

        if (filtered.Any())
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Número"); table.AddColumn("Clase");
            foreach (var s in filtered)
            {
                var cls = classMap.TryGetValue(s.IdClase, out var cn) ? cn : s.IdClase.ToString();
                table.AddRow(s.Id.Value.ToString(), Markup.Escape(s.Number.Value), Markup.Escape(cls));
            }
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine($"\n[grey]Total: {filtered.Count} asientos.[/]");
        }

        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task<int> SelectAirlineAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllAerolinesUseCase(new AerolineRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay aerolíneas registradas. Crea una primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona la aerolínea:").PageSize(10)
                .AddChoices(items.Select(a => $"{a.Id.Value}. {a.Name.Value} ({a.IATACode.Value})")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectModelAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllAircraftModelsUseCase(new AircraftModelRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay modelos registrados. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el modelo:").PageSize(10)
                .AddChoices(items.Select(m => $"{m.Id.Value}. {m.Name.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]REGISTRAR AERONAVE[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas registrar una aeronave?", true))
            return;
        var capacity = AnsiConsole.Prompt(
            new TextPrompt<int>("Capacidad total de pasajeros:")
                .Validate(v => v > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]La capacidad debe ser mayor a 0[/]")));
        try
        {
            var idAirline = await SelectAirlineAsync(ct);
            var idModel = await SelectModelAsync(ct);

            using var context = DbContextFactory.Create();
            // Importante: el agregado nuevo se crea con Id=0 y la BD lo genera al guardar.
            // Como aquí mapeamos agregado→entidad, necesitamos leer el Id generado desde la entidad.
            var aggregate = await new CreateAircraftUseCase(new AircraftRepository(context))
                .ExecuteAsync(capacity, idAirline, idModel, ct);

            var trackedEntity = new AircraftEntity
            {
                IdAircraft = 0,
                Capacity = aggregate.Capacity.Value,
                IdAirline = aggregate.IdAirline,
                IdModel = aggregate.IdModel
            };
            await context.Set<AircraftEntity>().AddAsync(trackedEntity, ct);
            await context.SaveChangesAsync(ct);
            var createdId = trackedEntity.IdAircraft;
            AnsiConsole.MarkupLine($"\n[green]Aeronave registrada con ID {createdId}, capacidad {aggregate.Capacity.Value}.[/]");

            // Ofrecer generación automática de asientos
            if (AnsiConsole.Confirm("\n¿Generar asientos automáticamente ahora?", true))
                await GenerateSeatsAsync(createdId, capacity, ct);
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task GenerateSeatsAsync(int idAircraft, int totalCapacity, CancellationToken ct)
    {
        // Carga clases disponibles para distribuir asientos
        using var classContext = DbContextFactory.Create();
        var seatClasses = await new GetAllSeatClassesUseCase(new SeatClassRepository(classContext)).ExecuteAsync(ct);
        if (!seatClasses.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay clases de asiento registradas. Agrega asientos manualmente desde este menú.[/]");
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

        if (!distribution.Any()) return;

        // Genera números de asiento: filas de 6 columnas (A-F)
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
        AnsiConsole.MarkupLine($"\n[green]Se generaron {distribution.Sum(d => d.count)} asientos correctamente.[/]");
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR AERONAVE[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la aeronave a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var capacity = AnsiConsole.Prompt(
            new TextPrompt<int>("Nueva capacidad:")
                .Validate(v => v > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]La capacidad debe ser mayor a 0[/]")));
        try
        {
            var idAirline = await SelectAirlineAsync(ct);
            var idModel = await SelectModelAsync(ct);
            using var context = DbContextFactory.Create();
            await new UpdateAircraftUseCase(new AircraftRepository(context)).ExecuteAsync(id, capacity, idAirline, idModel, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Aeronave actualizada correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR AERONAVE[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la aeronave a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar la aeronave con ID {id}? Esto también eliminará sus asientos."))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteAircraftUseCase(new AircraftRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Aeronave eliminada correctamente.[/]" : "\n[yellow]No se encontró la aeronave con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
