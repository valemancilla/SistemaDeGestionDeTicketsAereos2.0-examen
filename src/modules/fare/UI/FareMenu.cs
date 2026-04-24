using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.UI;

public sealed class FareMenu
{
    private const decimal MaxPrecioBaseCop = 15_000_000m;

    private static ValidationResult ValidatePrecioBase(decimal v)
    {
        if (v <= 0)
            return ValidationResult.Error("[red]El precio debe ser mayor a 0[/]");
        if (v > MaxPrecioBaseCop)
            return ValidationResult.Error($"[red]El precio no puede superar {MaxPrecioBaseCop:N0} COP[/]");
        return ValidationResult.Success();
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE TARIFAS[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(7)
                    .AddChoices("1. Crear tarifa", "2. Listar tarifas", "3. Actualizar tarifa",
                                "4. Eliminar tarifa", "0. Volver"));

            switch (option)
            {
                case "1. Crear tarifa":      await CreateAsync(ct); break;
                case "2. Listar tarifas":   await ListAsync(ct);   break;
                case "3. Actualizar tarifa": await UpdateAsync(ct); break;
                case "4. Eliminar tarifa":   await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var fares = await new GetAllFaresUseCase(new FareRepository(context)).ExecuteAsync(ct);
        var airlines = await new GetAllAerolinesUseCase(new AerolineRepository(context)).ExecuteAsync(ct);
        var airlineMap = airlines.ToDictionary(a => a.Id.Value, a => a.Name.Value);

        // Precios por clase (si la tarifa se creó con desglose por clase)
        var classNames = await context.Set<SeatClassEntity>()
            .AsNoTracking()
            .ToDictionaryAsync(sc => sc.IdClase, sc => sc.ClassName, ct);

        var pricesByFare = await context.Set<FareSeatClassPriceEntity>()
            .AsNoTracking()
            .ToListAsync(ct);

        string PricesSummary(int idFare)
        {
            var rows = pricesByFare
                .Where(x => x.IdFare == idFare)
                .OrderBy(x => x.IdClase)
                .Select(x =>
                {
                    var cls = classNames.TryGetValue(x.IdClase, out var n) ? n : $"Clase {x.IdClase}";
                    return $"{cls}: {x.Price:N0}";
                })
                .ToList();

            if (rows.Count == 0)
                return "-";

            // Evitar celdas enormes: mostrar hasta 3 clases y luego “+N”
            const int max = 3;
            if (rows.Count <= max)
                return string.Join(" | ", rows);

            return string.Join(" | ", rows.Take(max)) + $" | +{rows.Count - max}";
        }

        if (!fares.Any()) { AnsiConsole.MarkupLine("[yellow]No hay tarifas registradas.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre"); table.AddColumn("Precio Base");
            table.AddColumn("Válida Desde"); table.AddColumn("Válida Hasta"); table.AddColumn("Aerolínea"); table.AddColumn("Activa");
            table.AddColumn("Expira"); table.AddColumn("Precios por clase");
            foreach (var f in fares)
            {
                var airline = airlineMap.TryGetValue(f.IdAirline, out var an) ? an : f.IdAirline.ToString();
                var expiration = f.ExpirationDate.Value is DateOnly d ? d.ToString("yyyy-MM-dd") : "-";
                table.AddRow(f.Id.Value.ToString(), Markup.Escape(f.Name.Value),
                    f.BasePrice.Value.ToString("C2"),
                    f.ValidFrom.Value.ToString("yyyy-MM-dd"), f.ValidTo.Value.ToString("yyyy-MM-dd"),
                    Markup.Escape(airline), f.Active ? "[green]Sí[/]" : "[red]No[/]",
                    expiration,
                    Markup.Escape(PricesSummary(f.Id.Value)));
            }
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
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

    private static async Task<int> SelectAircraftAsync(int idAirline, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var aircrafts = await new GetAllAircraftsUseCase(new AircraftRepository(context)).ExecuteAsync(ct);
        var list = aircrafts.Where(a => a.IdAirline == idAirline).OrderBy(a => a.Id.Value).ToList();
        if (!list.Any())
            throw new InvalidOperationException($"No hay aeronaves registradas para la aerolínea ID {idAirline}.");

        var seatCounts = await context.Set<SeatEntity>()
            .AsNoTracking()
            .Where(s => list.Select(a => a.Id.Value).Contains(s.IdAircraft))
            .GroupBy(s => s.IdAircraft)
            .Select(g => new { IdAircraft = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.IdAircraft, x => x.Count, ct);

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecciona la aeronave (para detectar clases por sus asientos):")
                .PageSize(10)
                .AddChoices(list.Select(a =>
                {
                    var seats = seatCounts.TryGetValue(a.Id.Value, out var c) ? c : 0;
                    var seatsLabel = seats > 0 ? $"{seats} asiento(s)" : "[red]sin asientos[/]";
                    return $"{a.Id.Value}. Aeronave {a.Id.Value} — capacidad {a.Capacity.Value} — {seatsLabel}";
                })));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<List<(int idClase, string className, int count, decimal price)>> PromptPricesBySeatClassFromAircraftAsync(
        int idAircraft,
        CancellationToken ct)
    {
        using var context = DbContextFactory.Create();

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
            var aircraft = await context.Set<AircraftEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdAircraft == idAircraft, ct);

            var capacity = aircraft?.Capacity ?? 0;
            AnsiConsole.MarkupLine("[yellow]La aeronave no tiene asientos registrados.[/]");
            if (capacity <= 0)
                throw new InvalidOperationException("No se encontró la aeronave o su capacidad para generar asientos automáticamente.");

            if (!AnsiConsole.Confirm($"¿Deseas generarlos automáticamente ahora? (capacidad {capacity})", true))
                throw new InvalidOperationException("No se pueden asignar precios por clase sin asientos. Genera asientos o crea una tarifa con precio base.");

            await GenerateAircraftSeatsAsync(context, idAircraft, capacity, ct);
            await context.SaveChangesAsync(ct);

            seatGroups = await LoadSeatGroupsAsync();
            if (seatGroups.Count == 0)
                throw new InvalidOperationException("No se pudieron generar asientos. Intenta crearlos desde el menú de Aeronaves.");
        }

        var seatClassNames = await context.Set<SeatClassEntity>()
            .AsNoTracking()
            .Where(sc => seatGroups.Select(x => x.IdClase).Contains(sc.IdClase))
            .ToDictionaryAsync(sc => sc.IdClase, sc => sc.ClassName, ct);

        var result = new List<(int idClase, string className, int count, decimal price)>(seatGroups.Count);
        AnsiConsole.MarkupLine("\n[grey]Ingresa el precio por asiento para cada clase encontrada en la aeronave.[/]");
        foreach (var g in seatGroups.OrderBy(x => x.IdClase))
        {
            var clsName = seatClassNames.TryGetValue(g.IdClase, out var n) ? n : $"Clase {g.IdClase}";
            var price = AnsiConsole.Prompt(
                new TextPrompt<decimal>($"Precio por asiento — {Markup.Escape(clsName)} ({g.Count} asiento(s)) (COP):")
                    .Validate(ValidatePrecioBase));
            result.Add((g.IdClase, clsName, g.Count, price));
        }

        return result;
    }

    private static async Task GenerateAircraftSeatsAsync(AppDbContext context, int idAircraft, int totalCapacity, CancellationToken ct)
    {
        var seatClasses = await context.Set<SeatClassEntity>()
            .AsNoTracking()
            .OrderBy(sc => sc.IdClase)
            .ToListAsync(ct);

        if (seatClasses.Count == 0)
            throw new InvalidOperationException("No hay clases de asiento registradas. Crea clases primero (Administración → Clases de asiento).");

        AnsiConsole.MarkupLine($"\n[grey]Distribuye los {totalCapacity} asientos por clase:[/]");
        var distribution = new List<(int idClass, string className, int count)>();
        int remaining = totalCapacity;

        foreach (var cls in seatClasses)
        {
            if (remaining <= 0) break;
            var count = AnsiConsole.Ask<int>($"  Asientos de clase '[bold]{Markup.Escape(cls.ClassName)}[/]' (quedan {remaining}):");
            count = Math.Min(count, remaining);
            if (count > 0) distribution.Add((cls.IdClase, cls.ClassName, count));
            remaining -= count;
        }

        if (!distribution.Any())
            throw new InvalidOperationException("No se generaron asientos porque la distribución fue 0.");

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
    }

    private static (DateOnly validFrom, DateOnly validTo, DateOnly? expiration) AskDates()
    {
        var fromStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Válida desde (yyyy-MM-dd):")
                .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                    ? ValidationResult.Success() : ValidationResult.Error("[red]Formato inválido[/]")));
        var toStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Válida hasta (yyyy-MM-dd):")
                .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                    ? ValidationResult.Success() : ValidationResult.Error("[red]Formato inválido[/]")));
        var expStr = AnsiConsole.Prompt(
                new TextPrompt<string>("Fecha de expiración (yyyy-MM-dd, Enter para omitir):")
                    .AllowEmpty())
            .Trim();
        DateOnly? expiration = string.IsNullOrEmpty(expStr) ? null
            : DateOnly.TryParseExact(expStr, "yyyy-MM-dd", out var d) ? d : null;
        var from = DateOnly.ParseExact(fromStr, "yyyy-MM-dd");
        var to   = DateOnly.ParseExact(toStr,   "yyyy-MM-dd");
        if (from > to) throw new InvalidOperationException("La fecha 'válida desde' no puede ser posterior a 'válida hasta'.");
        return (from, to, expiration);
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR TARIFA[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear una tarifa?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre de la tarifa:");
        var (validFrom, validTo, expiration) = AskDates();
        try
        {
            var idAirline = await SelectAirlineAsync(ct);
            var active = AnsiConsole.Confirm("¿Tarifa activa?", true);

            var useByClass = AnsiConsole.Confirm("¿Asignar precio por clase (Económica/Ejecutiva/Primera) según los asientos de una aeronave?", true);
            decimal basePrice;
            List<(int idClase, string className, int count, decimal price)> byClass = new();
            int? idAircraft = null;

            if (useByClass)
            {
                idAircraft = await SelectAircraftAsync(idAirline, ct);
                byClass = await PromptPricesBySeatClassFromAircraftAsync(idAircraft.Value, ct);
                basePrice = byClass.Min(x => x.price);
            }
            else
            {
                basePrice = AnsiConsole.Prompt(
                    new TextPrompt<decimal>("Precio base (COP, máx. 15.000.000):")
                        .Validate(ValidatePrecioBase));
            }

            using var context = DbContextFactory.Create();
            var result = await new CreateFareUseCase(new FareRepository(context))
                .ExecuteAsync(name, basePrice, validFrom, validTo, expiration, idAirline, active, ct);
            await context.SaveChangesAsync(ct);

            // El agregado se crea con Id=0 (autogenerado por BD). Recuperamos el Id real para mostrarlo.
            var all = await new GetAllFaresUseCase(new FareRepository(context)).ExecuteAsync(ct);
            var createdId = all
                .Where(f =>
                    f.Name.Value == name &&
                    f.BasePrice.Value == basePrice &&
                    f.ValidFrom.Value == validFrom &&
                    f.ValidTo.Value == validTo &&
                    f.IdAirline == idAirline &&
                    f.Active == active)
                .OrderByDescending(f => f.Id.Value)
                .Select(f => f.Id.Value)
                .FirstOrDefault();

            if (useByClass && createdId > 0 && byClass.Count > 0)
            {
                var rows = byClass.Select(x => new FareSeatClassPriceEntity
                {
                    IdFare = createdId,
                    IdClase = x.idClase,
                    Price = x.price
                }).ToList();
                await context.Set<FareSeatClassPriceEntity>().AddRangeAsync(rows, ct);
                await context.SaveChangesAsync(ct);
            }

            AnsiConsole.MarkupLine($"\n[green]Tarifa '[bold]{Markup.Escape(result.Name.Value)}[/]' creada con ID {createdId}.[/]");
            if (useByClass && byClass.Count > 0)
            {
                AnsiConsole.MarkupLine($"[grey]Precio base guardado (mínimo entre clases):[/] [bold]{basePrice:N0}[/] COP");
            }
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR TARIFA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la tarifa a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        var price = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Nuevo precio base (COP, máx. 15.000.000):")
                .Validate(ValidatePrecioBase));
        var (validFrom, validTo, expiration) = AskDates();
        try
        {
            var idAirline = await SelectAirlineAsync(ct);
            var active = AnsiConsole.Confirm("¿Tarifa activa?", true);
            using var context = DbContextFactory.Create();
            await new UpdateFareUseCase(new FareRepository(context))
                .ExecuteAsync(id, name, price, validFrom, validTo, expiration, idAirline, active, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Tarifa actualizada correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR TARIFA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la tarifa a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar la tarifa con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteFareUseCase(new FareRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Tarifa eliminada correctamente.[/]" : "\n[yellow]No se encontró la tarifa con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }
}
