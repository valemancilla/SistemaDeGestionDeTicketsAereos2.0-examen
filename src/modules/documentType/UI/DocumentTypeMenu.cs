using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.UI;

public sealed class DocumentTypeMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE TIPOS DE DOCUMENTO[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear tipo", "2. Listar tipos", "3. Actualizar tipo", "4. Eliminar tipo", "0. Volver"));

            switch (option)
            {
                case "1. Crear tipo": await CreateAsync(ct); break;
                case "2. Listar tipos": await ListAsync(ct); break;
                case "3. Actualizar tipo": await UpdateAsync(ct); break;
                case "4. Eliminar tipo": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var items = await new GetAllDocumentTypesUseCase(new DocumentTypeRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) { AnsiConsole.MarkupLine("[yellow]No hay tipos de documento registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre");
            foreach (var d in items)
                table.AddRow(d.Id.Value.ToString(), Markup.Escape(d.Name.Value));
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR TIPO DE DOCUMENTO[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un tipo de documento?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre (ej: Cédula, Pasaporte, DNI):");
        try
        {
            using var context = DbContextFactory.Create();
            var result = await new CreateDocumentTypeUseCase(new DocumentTypeRepository(context)).ExecuteAsync(name, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllDocumentTypesUseCase(new DocumentTypeRepository(context)).ExecuteAsync(ct))
                .Where(d => d.Name.Value == name)
                .OrderByDescending(d => d.Id.Value)
                .Select(d => d.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Tipo '[bold]{Markup.Escape(result.Name.Value)}[/]' creado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR TIPO DE DOCUMENTO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del tipo a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateDocumentTypeUseCase(new DocumentTypeRepository(context)).ExecuteAsync(id, name, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Tipo de documento actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR TIPO DE DOCUMENTO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del tipo a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el tipo con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteDocumentTypeUseCase(new DocumentTypeRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Tipo eliminado correctamente.[/]" : "\n[yellow]No se encontró el tipo con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
