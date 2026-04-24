using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.UI;

public sealed class TimeZoneMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE ZONAS HORARIAS[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear zona", "2. Listar zonas", "3. Actualizar zona", "4. Eliminar zona", "0. Volver"));

            switch (option)
            {
                case "1. Crear zona":      await CreateAsync(ct); break;
                case "2. Listar zonas":    await ListAsync(ct);   break;
                case "3. Actualizar zona": await UpdateAsync(ct); break;
                case "4. Eliminar zona":   await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var items = await new GetAllTimeZonesUseCase(new TimeZoneRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) { AnsiConsole.MarkupLine("[yellow]No hay zonas horarias registradas.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre"); table.AddColumn("UTC Offset");
            foreach (var z in items)
                table.AddRow(z.Id.Value.ToString(), Markup.Escape(z.Name.Value), Markup.Escape(z.UTCOffset.Value));
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR ZONA HORARIA[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear una zona horaria?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre (ej: America/Bogota):");
        var offset = AnsiConsole.Ask<string>("UTC Offset (ej: -05:00):");
        try
        {
            using var context = DbContextFactory.Create();
            var result = await new CreateTimeZoneUseCase(new TimeZoneRepository(context)).ExecuteAsync(name, offset, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllTimeZonesUseCase(new TimeZoneRepository(context)).ExecuteAsync(ct))
                .Where(z => z.Name.Value == name && z.UTCOffset.Value == offset)
                .OrderByDescending(z => z.Id.Value)
                .Select(z => z.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Zona '[bold]{Markup.Escape(result.Name.Value)}[/]' creada con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR ZONA HORARIA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la zona a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        var offset = AnsiConsole.Ask<string>("Nuevo UTC Offset:");
        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateTimeZoneUseCase(new TimeZoneRepository(context)).ExecuteAsync(id, name, offset, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Zona horaria actualizada correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR ZONA HORARIA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la zona a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar la zona con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteTimeZoneUseCase(new TimeZoneRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Zona eliminada correctamente.[/]" : "\n[yellow]No se encontró la zona con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }
}
