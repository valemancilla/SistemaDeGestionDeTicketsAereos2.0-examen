using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.UI;

public sealed class AerolineMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE AEROLÍNEAS[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(7)
                    .AddChoices("1. Crear aerolínea", "2. Listar aerolíneas", "3. Actualizar aerolínea", "4. Activar / Desactivar", "5. Eliminar aerolínea", "0. Volver"));

            switch (option)
            {
                case "1. Crear aerolínea":         await CreateAsync(ct); break;
                case "2. Listar aerolíneas":      await ListAsync(ct);   break;
                case "3. Actualizar aerolínea":    await UpdateAsync(ct); break;
                case "4. Activar / Desactivar":    await ToggleActiveAsync(ct); break;
                case "5. Eliminar aerolínea":      await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var airlines = await new GetAllAerolinesUseCase(new AerolineRepository(context)).ExecuteAsync(ct);
        var countries = await new GetAllCountriesUseCase(new CountryRepository(context)).ExecuteAsync(ct);
        var countryMap = countries.ToDictionary(c => c.Id.Value, c => c.Name.Value);

        if (!airlines.Any()) { AnsiConsole.MarkupLine("[yellow]No hay aerolíneas registradas.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre"); table.AddColumn("IATA"); table.AddColumn("País"); table.AddColumn("Activa");
            foreach (var a in airlines)
            {
                var country = countryMap.TryGetValue(a.IdCountry, out var cn) ? cn : a.IdCountry.ToString();
                table.AddRow(a.Id.Value.ToString(), Markup.Escape(a.Name.Value), Markup.Escape(a.IATACode.Value),
                    Markup.Escape(country), a.Active ? "[green]Sí[/]" : "[red]No[/]");
            }
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task<int> SelectCountryAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var countries = await new GetAllCountriesUseCase(new CountryRepository(context)).ExecuteAsync(ct);
        if (!countries.Any()) throw new InvalidOperationException("No hay países registrados. Crea uno primero.");
        countries = countries.OrderBy(c => c.Id.Value).ToList();
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el país de origen:").PageSize(10)
                .AddChoices(countries.Select(c => $"{c.Id.Value}. {c.Name.Value} ({c.ISOCode.Value})")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR AEROLÍNEA[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear una aerolínea?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre de la aerolínea:");
        var iata = AnsiConsole.Ask<string>("Código IATA (2 letras, ej: AV, LA):");
        try
        {
            var idCountry = await SelectCountryAsync(ct);
            var active = AnsiConsole.Confirm("¿Aerolínea activa?", true);
            using var context = DbContextFactory.Create();
            var repo = new AerolineRepository(context);
            await new CreateAerolineUseCase(repo).ExecuteAsync(name, iata, idCountry, active, ct);
            await context.SaveChangesAsync(ct);

            // Tras SaveChanges el agregado en memoria sigue con Id=0; el IATA es único — recargar devuelve el Id real de la BD.
            var created = await repo.GetByIataCodeAsync(iata, ct)
                ?? throw new InvalidOperationException("No se pudo recuperar la aerolínea recién creada.");

            AnsiConsole.MarkupLine($"\n[green]Aerolínea '[bold]{Markup.Escape(created.Name.Value)}[/]' ({created.IATACode.Value}) creada con ID {created.Id.Value}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR AEROLÍNEA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la aerolínea a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        var iata = AnsiConsole.Ask<string>("Nuevo código IATA:");
        try
        {
            var idCountry = await SelectCountryAsync(ct);
            var active = AnsiConsole.Confirm("¿Aerolínea activa?", true);
            using var context = DbContextFactory.Create();
            await new UpdateAerolineUseCase(new AerolineRepository(context)).ExecuteAsync(id, name, iata, idCountry, active, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Aerolínea actualizada correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task ToggleActiveAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTIVAR / DESACTIVAR AEROLÍNEA[/]").Centered());
        // Muestra lista para seleccionar
        using var listContext = DbContextFactory.Create();
        var airlines = await new GetAllAerolinesUseCase(new AerolineRepository(listContext)).ExecuteAsync(ct);
        if (!airlines.Any()) { AnsiConsole.MarkupLine("[yellow]No hay aerolíneas registradas.[/]"); Console.ReadKey(); return; }

        var choices = airlines.Select(a => $"{a.Id.Value}. {a.Name.Value} ({a.IATACode.Value}) — {(a.Active ? "Activa" : "Inactiva")}").ToList();
        var selected = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Selecciona la aerolínea:").PageSize(10).AddChoices(choices));
        var selectedId = int.Parse(selected.Split(new char[] { '.' })[0]);
        var airline = airlines.First(a => a.Id.Value == selectedId);
        var newActive = !airline.Active;

        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateAerolineUseCase(new AerolineRepository(context)).ExecuteAsync(
                airline.Id.Value, airline.Name.Value, airline.IATACode.Value, airline.IdCountry, newActive, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine($"\n[green]Aerolínea '{Markup.Escape(airline.Name.Value)}' ahora está {(newActive ? "ACTIVA" : "INACTIVA")}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR AEROLÍNEA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la aerolínea a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar la aerolínea con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteAerolineUseCase(new AerolineRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Aerolínea eliminada correctamente.[/]" : "\n[yellow]No se encontró la aerolínea con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }
}
