using SistemaDeGestionDeTicketsAereos.src.modules.city.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.UI;

public sealed class CityMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE CIUDADES[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear ciudad", "2. Listar ciudades", "3. Actualizar ciudad", "4. Eliminar ciudad", "0. Volver"));

            switch (option)
            {
                case "1. Crear ciudad":      await CreateAsync(ct); break;
                case "2. Listar ciudades":   await ListAsync(ct);   break;
                case "3. Actualizar ciudad": await UpdateAsync(ct); break;
                case "4. Eliminar ciudad":   await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var cityRepo = new CityRepository(context);
        var countryRepo = new CountryRepository(context);
        var cities = await new GetAllCitiesUseCase(cityRepo).ExecuteAsync(ct);
        var countries = await new GetAllCountriesUseCase(countryRepo).ExecuteAsync(ct);
        var countryMap = countries.ToDictionary(c => c.Id.Value, c => c.Name.Value);

        if (!cities.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay ciudades registradas.[/]");
        }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID");
            table.AddColumn("Nombre");
            table.AddColumn("País");
            foreach (var c in cities)
            {
                var countryName = countryMap.TryGetValue(c.IdCountry, out var cn) ? cn : c.IdCountry.ToString();
                table.AddRow(c.Id.Value.ToString(), Markup.Escape(c.Name.Value), Markup.Escape(countryName));
            }
            AnsiConsole.Write(table);
        }

        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task<int> SelectCountryAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var countries = await new GetAllCountriesUseCase(new CountryRepository(context)).ExecuteAsync(ct);
        if (!countries.Any())
            throw new InvalidOperationException("No hay países registrados. Crea un país primero.");

        var choices = countries.Select(c => $"{c.Id.Value}. {c.Name.Value} ({c.ISOCode.Value})").ToList();
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecciona el país:")
                .PageSize(10)
                .AddChoices(choices));
        return int.Parse(selected.Split('.')[0]);
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR CIUDAD[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear una ciudad?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre de la ciudad:");

        try
        {
            var idCountry = await SelectCountryAsync(ct);
            using var context = DbContextFactory.Create();
            var useCase = new CreateCityUseCase(new CityRepository(context));
            var result = await useCase.ExecuteAsync(name, idCountry, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllCitiesUseCase(new CityRepository(context)).ExecuteAsync(ct))
                .Where(c => c.Name.Value == name && c.IdCountry == idCountry)
                .OrderByDescending(c => c.Id.Value)
                .Select(c => c.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Ciudad '[bold]{Markup.Escape(result.Name.Value)}[/]' creada con ID {createdId}.[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR CIUDAD[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la ciudad a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");

        try
        {
            var idCountry = await SelectCountryAsync(ct);
            using var context = DbContextFactory.Create();
            var useCase = new UpdateCityUseCase(new CityRepository(context));
            await useCase.ExecuteAsync(id, name, idCountry, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine($"\n[green]Ciudad actualizada correctamente.[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR CIUDAD[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la ciudad a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;

        if (!AnsiConsole.Confirm($"¿Confirma eliminar la ciudad con ID {id}?"))
        {
            AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]");
            Console.ReadKey();
            return;
        }

        try
        {
            using var context = DbContextFactory.Create();
            var useCase = new DeleteCityUseCase(new CityRepository(context));
            var deleted = await useCase.ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted
                ? "\n[green]Ciudad eliminada correctamente.[/]"
                : "\n[yellow]No se encontró la ciudad con ese ID.[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }
}
