using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.UI;

public sealed class EmployeeMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE EMPLEADOS[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(7)
                    .AddChoices("1. Registrar empleado", "2. Listar empleados",
                                "3. Actualizar empleado", "4. Eliminar empleado", "0. Volver"));

            switch (option)
            {
                case "1. Registrar empleado": await CreateAsync(ct); break;
                case "2. Listar empleados": await ListAsync(ct); break;
                case "3. Actualizar empleado": await UpdateAsync(ct); break;
                case "4. Eliminar empleado": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var employees = await new GetAllEmployeesUseCase(new EmployeeRepository(context)).ExecuteAsync(ct);
        var persons = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
        var airlines = await new GetAllAerolinesUseCase(new AerolineRepository(context)).ExecuteAsync(ct);
        var roles = await new GetAllEmployeeRolesUseCase(new EmployeeRoleRepository(context)).ExecuteAsync(ct);
        var personMap = persons.ToDictionary(p => p.Id.Value, p => $"{p.FirstName.Value} {p.LastName.Value}");
        var airlineMap = airlines.ToDictionary(a => a.Id.Value, a => a.Name.Value);
        var roleMap = roles.ToDictionary(r => r.Id.Value, r => r.Name.Value);

        if (!employees.Any()) { AnsiConsole.MarkupLine("[yellow]No hay empleados registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Persona"); table.AddColumn("Aerolínea"); table.AddColumn("Rol");
            foreach (var e in employees)
            {
                var person = personMap.TryGetValue(e.IdPerson, out var pn) ? pn : e.IdPerson.ToString();
                var airline = airlineMap.TryGetValue(e.IdAirline, out var an) ? an : e.IdAirline.ToString();
                var role = roleMap.TryGetValue(e.IdRole, out var rn) ? rn : e.IdRole.ToString();
                table.AddRow(e.Id.Value.ToString(), Markup.Escape(person), Markup.Escape(airline), Markup.Escape(role));
            }
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task<int> SelectPersonAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay personas registradas. Crea una primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona la persona:").PageSize(10)
                .AddChoices(items.Select(p => $"{p.Id.Value}. {p.FirstName.Value} {p.LastName.Value} ({p.DocumentNumber.Value})")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectAirlineAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllAerolinesUseCase(new AerolineRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay aerolíneas registradas. Crea una primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona la aerolínea:").PageSize(10)
                .AddChoices(items.Select(a => $"{a.Id.Value}. {a.Name.Value} ({a.IATACode.Value})")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectEmployeeRoleAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllEmployeeRolesUseCase(new EmployeeRoleRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay roles de empleado registrados. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el rol:").PageSize(10)
                .AddChoices(items.Select(r => $"{r.Id.Value}. {r.Name.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]REGISTRAR EMPLEADO[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas registrar un empleado?", true))
            return;
        try
        {
            var idPerson = await SelectPersonAsync(ct);
            var idAirline = await SelectAirlineAsync(ct);
            var idRole = await SelectEmployeeRoleAsync(ct);
            using var context = DbContextFactory.Create();
            var result = await new CreateEmployeeUseCase(new EmployeeRepository(context))
                .ExecuteAsync(idPerson, idAirline, idRole, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllEmployeesUseCase(new EmployeeRepository(context)).ExecuteAsync(ct))
                .Where(e => e.IdPerson == idPerson && e.IdAirline == idAirline && e.IdRole == idRole)
                .OrderByDescending(e => e.Id.Value)
                .Select(e => e.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Empleado registrado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR EMPLEADO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del empleado a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        try
        {
            var idPerson = await SelectPersonAsync(ct);
            var idAirline = await SelectAirlineAsync(ct);
            var idRole = await SelectEmployeeRoleAsync(ct);
            using var context = DbContextFactory.Create();
            await new UpdateEmployeeUseCase(new EmployeeRepository(context))
                .ExecuteAsync(id, idPerson, idAirline, idRole, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Empleado actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR EMPLEADO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del empleado a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el empleado con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteEmployeeUseCase(new EmployeeRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Empleado eliminado correctamente.[/]" : "\n[yellow]No se encontró el empleado con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
