using SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.UI;

public sealed class PersonMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE PERSONAS[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(8)
                    .AddChoices("1. Crear persona", "2. Listar personas", "3. Actualizar persona",
                                "4. Eliminar persona", "0. Volver"));

            switch (option)
            {
                case "1. Crear persona":         await CreateAsync(ct);         break;
                case "2. Listar personas":      await ListAsync(ct);           break;
                case "3. Actualizar persona":    await UpdateAsync(ct);         break;
                case "4. Eliminar persona":      await DeleteAsync(ct);         break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var persons = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
        var countries = await new GetAllCountriesUseCase(new CountryRepository(context)).ExecuteAsync(ct);
        var docTypes = await new GetAllDocumentTypesUseCase(new DocumentTypeRepository(context)).ExecuteAsync(ct);
        var genders = await new GetAllGendersUseCase(new GenderRepository(context)).ExecuteAsync(ct);
        var countryMap = countries.ToDictionary(c => c.Id.Value, c => c.Name.Value);
        var docMap = docTypes.ToDictionary(d => d.Id.Value, d => d.Name.Value);
        var genderMap = genders.ToDictionary(g => g.Id.Value, g => g.Description.Value);

        if (!persons.Any()) { AnsiConsole.MarkupLine("[yellow]No hay personas registradas.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre"); table.AddColumn("Apellido");
            table.AddColumn("F.Nacimiento"); table.AddColumn("Doc"); table.AddColumn("Tipo Doc");
            table.AddColumn("Género"); table.AddColumn("País");
            foreach (var p in persons)
            {
                var country = countryMap.TryGetValue(p.IdCountry, out var cn) ? cn : p.IdCountry.ToString();
                var doc = docMap.TryGetValue(p.IdDocumentType, out var dn) ? dn : p.IdDocumentType.ToString();
                var gender = genderMap.TryGetValue(p.IdGender, out var gn) ? gn : p.IdGender.ToString();
                table.AddRow(p.Id.Value.ToString(), Markup.Escape(p.FirstName.Value),
                    Markup.Escape(p.LastName.Value), p.BirthDate.Value.ToString("yyyy-MM-dd"),
                    Markup.Escape(p.DocumentNumber.Value), Markup.Escape(doc),
                    Markup.Escape(gender), Markup.Escape(country));
            }
            AnsiConsole.Write(table);
        }
        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task<int> SelectDocumentTypeAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllDocumentTypesUseCase(new DocumentTypeRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay tipos de documento. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Tipo de documento:").PageSize(10)
                .AddChoices(items.Select(d => $"{d.Id.Value}. {d.Name.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectGenderAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllGendersUseCase(new GenderRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay géneros registrados. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Género:").PageSize(10)
                .AddChoices(items.Select(g => $"{g.Id.Value}. {g.Description.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectCountryAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllCountriesUseCase(new CountryRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay países registrados. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("País de nacionalidad:").PageSize(10)
                .AddChoices(items.Select(c => $"{c.Id.Value}. {c.Name.Value} ({c.ISOCode.Value})")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR PERSONA[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear una persona?", true))
            return;
        var firstName = AnsiConsole.Ask<string>("Nombre(s):");
        var lastName = AnsiConsole.Ask<string>("Apellido(s):");
        var birthDateStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Fecha de nacimiento (yyyy-MM-dd):")
                .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido. Use yyyy-MM-dd[/]")));
        var birthDate = DateOnly.ParseExact(birthDateStr, "yyyy-MM-dd");
        var documentNumber = AnsiConsole.Ask<string>("Número de documento:");
        try
        {
            var idDocType = await SelectDocumentTypeAsync(ct);
            var idGender = await SelectGenderAsync(ct);
            var idCountry = await SelectCountryAsync(ct);
            var docNorm = PersonDocumentNumber.Create(documentNumber).Value;
            using var context = DbContextFactory.Create();
            var result = await new CreatePersonUseCase(new PersonRepository(context))
                .ExecuteAsync(firstName, lastName, birthDate, documentNumber, idDocType, idGender, idCountry, null, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct))
                .Where(p =>
                    p.FirstName.Value == firstName &&
                    p.LastName.Value == lastName &&
                    p.BirthDate.Value == birthDate &&
                    p.DocumentNumber.Value == docNorm &&
                    p.IdDocumentType == idDocType &&
                    p.IdGender == idGender &&
                    p.IdCountry == idCountry)
                .OrderByDescending(p => p.Id.Value)
                .Select(p => p.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Persona '[bold]{Markup.Escape(result.FirstName.Value)} {Markup.Escape(result.LastName.Value)}[/]' creada con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR PERSONA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la persona a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var firstName = AnsiConsole.Ask<string>("Nuevo nombre(s):");
        var lastName = AnsiConsole.Ask<string>("Nuevo apellido(s):");
        var birthDateStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Nueva fecha de nacimiento (yyyy-MM-dd):")
                .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Formato inválido. Use yyyy-MM-dd[/]")));
        var birthDate = DateOnly.ParseExact(birthDateStr, "yyyy-MM-dd");
        var documentNumber = AnsiConsole.Ask<string>("Nuevo número de documento:");
        try
        {
            var idDocType = await SelectDocumentTypeAsync(ct);
            var idGender = await SelectGenderAsync(ct);
            var idCountry = await SelectCountryAsync(ct);
            using var context = DbContextFactory.Create();
            await new UpdatePersonUseCase(new PersonRepository(context))
                .ExecuteAsync(id, firstName, lastName, birthDate, documentNumber, idDocType, idGender, idCountry, null, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Persona actualizada correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR PERSONA[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID de la persona a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar la persona con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeletePersonUseCase(new PersonRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Persona eliminada correctamente.[/]" : "\n[yellow]No se encontró la persona con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]"); Console.ReadKey();
    }
}
