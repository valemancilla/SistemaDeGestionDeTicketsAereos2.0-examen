using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.UI;

public sealed class CheckInChannelMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE CANALES DE CHECK-IN[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear canal", "2. Listar canales", "3. Actualizar canal", "4. Eliminar canal", "0. Volver"));

            switch (option)
            {
                case "1. Crear canal":       await CreateAsync(ct); break;
                case "2. Listar canales":    await ListAsync(ct);   break;
                case "3. Actualizar canal":  await UpdateAsync(ct); break;
                case "4. Eliminar canal":    await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var items = await new GetAllCheckInChannelsUseCase(new CheckInChannelRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) { AnsiConsole.MarkupLine("[yellow]No hay canales de check-in registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre");
            foreach (var c in items)
                table.AddRow(c.Id.Value.ToString(), Markup.Escape(c.Name.Value));
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR CANAL DE CHECK-IN[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un canal de check-in?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre (ej: Web, App móvil, Mostrador):");
        try
        {
            using var context = DbContextFactory.Create();
            var result = await new CreateCheckInChannelUseCase(new CheckInChannelRepository(context)).ExecuteAsync(name, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllCheckInChannelsUseCase(new CheckInChannelRepository(context)).ExecuteAsync(ct))
                .Where(c => c.Name.Value == name)
                .OrderByDescending(c => c.Id.Value)
                .Select(c => c.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Canal '[bold]{Markup.Escape(result.Name.Value)}[/]' creado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR CANAL[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del canal a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateCheckInChannelUseCase(new CheckInChannelRepository(context)).ExecuteAsync(id, name, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Canal actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR CANAL[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del canal a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el canal con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteCheckInChannelUseCase(new CheckInChannelRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Canal eliminado correctamente.[/]" : "\n[yellow]No se encontró el canal con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }
}
