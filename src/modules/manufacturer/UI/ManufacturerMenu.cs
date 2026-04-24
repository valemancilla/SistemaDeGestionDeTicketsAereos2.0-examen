using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.UI;

public sealed class ManufacturerMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE FABRICANTES[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear fabricante", "2. Listar fabricantes", "3. Actualizar fabricante", "4. Eliminar fabricante", "0. Volver"));

            switch (option)
            {
                case "1. Crear fabricante": await CreateAsync(ct); break;
                case "2. Listar fabricantes": await ListAsync(ct); break;
                case "3. Actualizar fabricante": await UpdateAsync(ct); break;
                case "4. Eliminar fabricante": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var items = await new GetAllManufacturersUseCase(new ManufacturerRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) { AnsiConsole.MarkupLine("[yellow]No hay fabricantes registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre");
            foreach (var m in items)
                table.AddRow(m.Id.Value.ToString(), Markup.Escape(m.Name.Value));
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR FABRICANTE[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un fabricante?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre (ej: Boeing, Airbus, Embraer):");
        try
        {
            using var context = DbContextFactory.Create();
            var result = await new CreateManufacturerUseCase(new ManufacturerRepository(context)).ExecuteAsync(name, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllManufacturersUseCase(new ManufacturerRepository(context)).ExecuteAsync(ct))
                .Where(m => m.Name.Value == name)
                .OrderByDescending(m => m.Id.Value)
                .Select(m => m.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Fabricante '[bold]{Markup.Escape(result.Name.Value)}[/]' creado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR FABRICANTE[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del fabricante a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateManufacturerUseCase(new ManufacturerRepository(context)).ExecuteAsync(id, name, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Fabricante actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR FABRICANTE[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del fabricante a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el fabricante con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteManufacturerUseCase(new ManufacturerRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Fabricante eliminado correctamente.[/]" : "\n[yellow]No se encontró el fabricante con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
