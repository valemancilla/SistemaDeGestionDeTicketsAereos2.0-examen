using SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.UI;

public sealed class CountryMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE PAÍSES[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear país", "2. Listar países", "3. Actualizar país", "4. Eliminar país", "0. Volver"));

            switch (option)
            {
                case "1. Crear país": await CreateAsync(ct); break;
                case "2. Listar países": await ListAsync(ct); break;
                case "3. Actualizar país": await UpdateAsync(ct); break;
                case "4. Eliminar país": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var repo = new CountryRepository(context);
        var useCase = new GetAllCountriesUseCase(repo);
        var items = await useCase.ExecuteAsync(ct);

        if (!items.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay países registrados.[/]");
        }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID");
            table.AddColumn("Nombre");
            table.AddColumn("Código ISO");
            foreach (var c in items)
                table.AddRow(c.Id.Value.ToString(), Markup.Escape(c.Name.Value), Markup.Escape(c.ISOCode.Value));
            AnsiConsole.Write(table);
        }

        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR PAÍS[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un país?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre del país:");
        var iso = AnsiConsole.Ask<string>("Código ISO (2 letras, ej: CO):");

        try
        {
            using var context = DbContextFactory.Create();
            var repo = new CountryRepository(context);
            var useCase = new CreateCountryUseCase(repo);
            var result = await useCase.ExecuteAsync(name, iso, ct);
            await context.SaveChangesAsync(ct);

            // El agregado se crea con Id=0 (autogenerado por BD). Recuperamos el Id real para mostrarlo.
            var createdId = await context.Set<CountryEntity>()
                .AsNoTracking()
                .Where(c => c.Name == name && c.ISOCode == iso)
                .OrderByDescending(c => c.IdCountry)
                .Select(c => c.IdCountry)
                .FirstOrDefaultAsync(ct);

            AnsiConsole.MarkupLine($"\n[green]País '[bold]{Markup.Escape(result.Name.Value)}[/]' creado con ID {createdId}.[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR PAÍS[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del país a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        var iso = AnsiConsole.Ask<string>("Nuevo código ISO:");

        try
        {
            using var context = DbContextFactory.Create();
            var repo = new CountryRepository(context);
            var useCase = new UpdateCountryUseCase(repo);
            await useCase.ExecuteAsync(id, name, iso, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine($"\n[green]País actualizado correctamente.[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR PAÍS[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del país a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;

        if (!AnsiConsole.Confirm($"¿Confirma eliminar el país con ID {id}?"))
        {
            AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]");
            Console.ReadKey();
            return;
        }

        try
        {
            using var context = DbContextFactory.Create();
            var repo = new CountryRepository(context);
            var useCase = new DeleteCountryUseCase(repo);
            var deleted = await useCase.ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted
                ? "\n[green]País eliminado correctamente.[/]"
                : "\n[yellow]No se encontró el país con ese ID.[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
