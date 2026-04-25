using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.UI;

public sealed class SeatClassMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE CLASES DE ASIENTO[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear clase", "2. Listar clases", "3. Actualizar clase", "4. Eliminar clase", "0. Volver"));

            switch (option)
            {
                case "1. Crear clase": await CreateAsync(ct); break;
                case "2. Listar clases": await ListAsync(ct); break;
                case "3. Actualizar clase": await UpdateAsync(ct); break;
                case "4. Eliminar clase": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var items = await new GetAllSeatClassesUseCase(new SeatClassRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) { AnsiConsole.MarkupLine("[yellow]No hay clases de asiento registradas.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre");
            foreach (var s in items)
                table.AddRow(s.Id.Value.ToString(), Markup.Escape(s.Name.Value));
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR CLASE DE ASIENTO[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear una clase de asiento?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre (ej: Económica, Business, Primera Clase):");
        try
        {
            using var context = DbContextFactory.Create();
            var result = await new CreateSeatClassUseCase(new SeatClassRepository(context)).ExecuteAsync(name, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllSeatClassesUseCase(new SeatClassRepository(context)).ExecuteAsync(ct))
                .Where(s => s.Name.Value == name)
                .OrderByDescending(s => s.Id.Value)
                .Select(s => s.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Clase '[bold]{Markup.Escape(result.Name.Value)}[/]' creada con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR CLASE DE ASIENTO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la clase a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateSeatClassUseCase(new SeatClassRepository(context)).ExecuteAsync(id, name, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Clase de asiento actualizada correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR CLASE DE ASIENTO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la clase a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar la clase con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteSeatClassUseCase(new SeatClassRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Clase eliminada correctamente.[/]" : "\n[yellow]No se encontró la clase con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
