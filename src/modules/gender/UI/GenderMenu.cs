using SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.UI;

public sealed class GenderMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE GÉNEROS[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear género", "2. Listar géneros", "3. Actualizar género", "4. Eliminar género", "0. Volver"));

            switch (option)
            {
                case "1. Crear género":     await CreateAsync(ct); break;
                case "2. Listar géneros":   await ListAsync(ct);   break;
                case "3. Actualizar género":await UpdateAsync(ct); break;
                case "4. Eliminar género":  await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var items = await new GetAllGendersUseCase(new GenderRepository(context)).ExecuteAsync(ct);

        if (!items.Any()) { AnsiConsole.MarkupLine("[yellow]No hay géneros registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Descripción");
            foreach (var g in items)
                table.AddRow(g.Id.Value.ToString(), Markup.Escape(g.Description.Value));
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR GÉNERO[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un género?", true))
            return;
        var description = AnsiConsole.Ask<string>("Descripción (ej: Masculino, Femenino):");
        try
        {
            using var context = DbContextFactory.Create();
            var result = await new CreateGenderUseCase(new GenderRepository(context)).ExecuteAsync(description, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllGendersUseCase(new GenderRepository(context)).ExecuteAsync(ct))
                .Where(g => g.Description.Value == description)
                .OrderByDescending(g => g.Id.Value)
                .Select(g => g.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Género '[bold]{Markup.Escape(result.Description.Value)}[/]' creado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR GÉNERO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del género a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var description = AnsiConsole.Ask<string>("Nueva descripción:");
        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateGenderUseCase(new GenderRepository(context)).ExecuteAsync(id, description, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Género actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR GÉNERO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del género a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el género con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteGenderUseCase(new GenderRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Género eliminado correctamente.[/]" : "\n[yellow]No se encontró el género con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }
}
