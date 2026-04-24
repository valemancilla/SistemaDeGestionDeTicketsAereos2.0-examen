using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.UI;

public sealed class AircraftModelMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE MODELOS DE AERONAVE[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear modelo", "2. Listar modelos", "3. Actualizar modelo", "4. Eliminar modelo", "0. Volver"));

            switch (option)
            {
                case "1. Crear modelo":      await CreateAsync(ct); break;
                case "2. Listar modelos":    await ListAsync(ct);   break;
                case "3. Actualizar modelo": await UpdateAsync(ct); break;
                case "4. Eliminar modelo":   await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var models = await new GetAllAircraftModelsUseCase(new AircraftModelRepository(context)).ExecuteAsync(ct);
        var manufacturers = await new GetAllManufacturersUseCase(new ManufacturerRepository(context)).ExecuteAsync(ct);
        var mfMap = manufacturers.ToDictionary(m => m.Id.Value, m => m.Name.Value);

        if (!models.Any()) { AnsiConsole.MarkupLine("[yellow]No hay modelos registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre"); table.AddColumn("Fabricante");
            foreach (var m in models)
            {
                var mfName = mfMap.TryGetValue(m.IdManufacturer, out var n) ? n : m.IdManufacturer.ToString();
                table.AddRow(m.Id.Value.ToString(), Markup.Escape(m.Name.Value), Markup.Escape(mfName));
            }
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task<int> SelectManufacturerAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllManufacturersUseCase(new ManufacturerRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay fabricantes registrados. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el fabricante:").PageSize(10)
                .AddChoices(items.Select(m => $"{m.Id.Value}. {m.Name.Value}")));
        return int.Parse(selected.Split('.')[0]);
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR MODELO DE AERONAVE[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un modelo?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre del modelo (ej: Boeing 737, Airbus A320):");
        try
        {
            var idManufacturer = await SelectManufacturerAsync(ct);
            using var context = DbContextFactory.Create();
            var result = await new CreateAircraftModelUseCase(new AircraftModelRepository(context)).ExecuteAsync(name, idManufacturer, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllAircraftModelsUseCase(new AircraftModelRepository(context)).ExecuteAsync(ct))
                .Where(m => m.Name.Value == name && m.IdManufacturer == idManufacturer)
                .OrderByDescending(m => m.Id.Value)
                .Select(m => m.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Modelo '[bold]{Markup.Escape(result.Name.Value)}[/]' creado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR MODELO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del modelo a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        try
        {
            var idManufacturer = await SelectManufacturerAsync(ct);
            using var context = DbContextFactory.Create();
            await new UpdateAircraftModelUseCase(new AircraftModelRepository(context)).ExecuteAsync(id, name, idManufacturer, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Modelo actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR MODELO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del modelo a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el modelo con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteAircraftModelUseCase(new AircraftModelRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Modelo eliminado correctamente.[/]" : "\n[yellow]No se encontró el modelo con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }
}
