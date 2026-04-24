using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.UI;

public sealed class SystemStatusMenu
{
    private static readonly string[] EntityTypes = ["Flight", "Booking", "Ticket", "CheckIn", "Baggage", "Payment"];

    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE ESTADOS DEL SISTEMA[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear estado", "2. Listar estados", "3. Actualizar estado", "4. Eliminar estado", "0. Volver"));

            switch (option)
            {
                case "1. Crear estado":      await CreateAsync(ct); break;
                case "2. Listar estados":    await ListAsync(ct);   break;
                case "3. Actualizar estado": await UpdateAsync(ct); break;
                case "4. Eliminar estado":   await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var items = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) { AnsiConsole.MarkupLine("[yellow]No hay estados registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre"); table.AddColumn("Tipo de entidad");
            foreach (var s in items)
                table.AddRow(s.Id.Value.ToString(), Markup.Escape(s.Name.Value), Markup.Escape(s.EntityType.Value));
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR ESTADO DEL SISTEMA[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un estado del sistema?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre (ej: Programado, Cancelado, Completado):");
        var entityType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("¿A qué tipo de entidad aplica este estado?")
                .AddChoices(EntityTypes));
        try
        {
            using var context = DbContextFactory.Create();
            var result = await new CreateSystemStatusUseCase(new SystemStatusRepository(context)).ExecuteAsync(name, entityType, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct))
                .Where(s => s.Name.Value == name && s.EntityType.Value == entityType)
                .OrderByDescending(s => s.Id.Value)
                .Select(s => s.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Estado '[bold]{Markup.Escape(result.Name.Value)}[/]' ({result.EntityType.Value}) creado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR ESTADO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del estado a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        var entityType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Nuevo tipo de entidad:")
                .AddChoices(EntityTypes));
        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateSystemStatusUseCase(new SystemStatusRepository(context)).ExecuteAsync(id, name, entityType, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Estado actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR ESTADO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del estado a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el estado con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteSystemStatusUseCase(new SystemStatusRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Estado eliminado correctamente.[/]" : "\n[yellow]No se encontró el estado con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }
}
