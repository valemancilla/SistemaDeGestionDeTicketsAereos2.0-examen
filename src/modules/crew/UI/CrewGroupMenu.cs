using SistemaDeGestionDeTicketsAereos.src.modules.crew.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.UI;

public sealed class CrewGroupMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]TRIPULACIONES (GRUPOS Y MIEMBROS)[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(5)
                    .AddChoices("1. Grupos", "2. Miembros", "0. Volver"));

            switch (option)
            {
                case "1. Grupos": await RunGroupsMenuAsync(ct); break;
                case "2. Miembros": await RunMembersMenuAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task RunGroupsMenuAsync(CancellationToken ct)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GRUPOS DE TRIPULACIÓN[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices(
                        "1. Crear grupo",
                        "2. Listar grupos",
                        "3. Actualizar grupo",
                        "4. Eliminar grupo",
                        "0. Volver"));

            switch (option)
            {
                case "1. Crear grupo": await CreateCrewAsync(ct); break;
                case "2. Listar grupos": await ListCrewsAsync(ct); break;
                case "3. Actualizar grupo": await UpdateCrewAsync(ct); break;
                case "4. Eliminar grupo": await DeleteCrewAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task RunMembersMenuAsync(CancellationToken ct)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]MIEMBROS DE TRIPULACIÓN[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices(
                        "1. Asignar empleado a grupo",
                        "2. Ver miembros por grupo",
                        "3. Listar todos los miembros",
                        "4. Eliminar miembro",
                        "0. Volver"));

            switch (option)
            {
                case "1. Asignar empleado a grupo": await CreateMemberAsync(ct); break;
                case "2. Ver miembros por grupo": await ListMembersByCrewAsync(ct); break;
                case "3. Listar todos los miembros": await ListMembersAsync(ct); break;
                case "4. Eliminar miembro": await DeleteMemberAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListCrewsAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var crews = await new GetAllCrewsUseCase(new CrewRepository(context)).ExecuteAsync(ct);
        if (!crews.Any()) { AnsiConsole.MarkupLine("[yellow]No hay grupos de tripulación registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre de Grupo");
            foreach (var c in crews)
                table.AddRow(c.Id.Value.ToString(), Markup.Escape(c.GroupName.Value));
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task CreateCrewAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR GRUPO DE TRIPULACIÓN[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un grupo de tripulación?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre del grupo:");
        try
        {
            using var context = DbContextFactory.Create();
            var result = await new CreateCrewUseCase(new CrewRepository(context)).ExecuteAsync(name, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllCrewsUseCase(new CrewRepository(context)).ExecuteAsync(ct))
                .Where(c => c.GroupName.Value == name)
                .OrderByDescending(c => c.Id.Value)
                .Select(c => c.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Grupo '[bold]{Markup.Escape(result.GroupName.Value)}[/]' creado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateCrewAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR GRUPO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del grupo a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateCrewUseCase(new CrewRepository(context)).ExecuteAsync(id, name, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Grupo actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteCrewAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR GRUPO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del grupo a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el grupo con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteCrewUseCase(new CrewRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Grupo eliminado correctamente.[/]" : "\n[yellow]No se encontró el grupo con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task ListMembersAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var members = await new GetAllCrewMembersUseCase(new CrewMemberRepository(context)).ExecuteAsync(ct);
        var crews = await new GetAllCrewsUseCase(new CrewRepository(context)).ExecuteAsync(ct);
        var employees = await new GetAllEmployeesUseCase(new EmployeeRepository(context)).ExecuteAsync(ct);
        var persons = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
        var roles = await new GetAllEmployeeRolesUseCase(new EmployeeRoleRepository(context)).ExecuteAsync(ct);
        var crewMap = crews.ToDictionary(c => c.Id.Value, c => c.GroupName.Value);
        var personMap = persons.ToDictionary(p => p.Id.Value, p => $"{p.FirstName.Value} {p.LastName.Value}");
        var empPersonMap = employees.ToDictionary(e => e.Id.Value, e =>
            personMap.TryGetValue(e.IdPerson, out var pn) ? pn : e.IdPerson.ToString());
        var roleMap = roles.ToDictionary(r => r.Id.Value, r => r.Name.Value);

        if (!members.Any()) { AnsiConsole.MarkupLine("[yellow]No hay miembros registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Grupo"); table.AddColumn("Empleado"); table.AddColumn("Rol");
            foreach (var m in members)
            {
                var crew = crewMap.TryGetValue(m.IdCrew, out var cn) ? cn : m.IdCrew.ToString();
                var emp = empPersonMap.TryGetValue(m.IdEmployee, out var en) ? en : m.IdEmployee.ToString();
                var role = roleMap.TryGetValue(m.IdRole, out var rn) ? rn : m.IdRole.ToString();
                table.AddRow(m.Id.Value.ToString(), Markup.Escape(crew), Markup.Escape(emp), Markup.Escape(role));
            }
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task ListMembersByCrewAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]MIEMBROS POR GRUPO[/]").Centered());
        try
        {
            var idCrew = await SelectCrewAsync(ct);
            using var context = DbContextFactory.Create();
            var members = await new GetAllCrewMembersUseCase(new CrewMemberRepository(context)).ExecuteAsync(ct);
            var crews = await new GetAllCrewsUseCase(new CrewRepository(context)).ExecuteAsync(ct);
            var employees = await new GetAllEmployeesUseCase(new EmployeeRepository(context)).ExecuteAsync(ct);
            var persons = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
            var roles = await new GetAllEmployeeRolesUseCase(new EmployeeRoleRepository(context)).ExecuteAsync(ct);

            var crewMap = crews.ToDictionary(c => c.Id.Value, c => c.GroupName.Value);
            var personMap = persons.ToDictionary(p => p.Id.Value, p => $"{p.FirstName.Value} {p.LastName.Value}");
            var empPersonMap = employees.ToDictionary(e => e.Id.Value, e =>
                personMap.TryGetValue(e.IdPerson, out var pn) ? pn : e.IdPerson.ToString());
            var roleMap = roles.ToDictionary(r => r.Id.Value, r => r.Name.Value);

            var filtered = members.Where(m => m.IdCrew == idCrew).ToList();
            var crewName = crewMap.TryGetValue(idCrew, out var cn) ? cn : idCrew.ToString();

            if (!filtered.Any())
            {
                AnsiConsole.MarkupLine($"[yellow]El grupo '{Markup.Escape(crewName)}' no tiene miembros.[/]");
            }
            else
            {
                var table = new Table().Border(TableBorder.Rounded);
                table.AddColumn("ID"); table.AddColumn("Empleado"); table.AddColumn("Rol");
                foreach (var m in filtered)
                {
                    var emp = empPersonMap.TryGetValue(m.IdEmployee, out var en) ? en : m.IdEmployee.ToString();
                    var role = roleMap.TryGetValue(m.IdRole, out var rn) ? rn : m.IdRole.ToString();
                    table.AddRow(m.Id.Value.ToString(), Markup.Escape(emp), Markup.Escape(role));
                }
                AnsiConsole.Write(new Rule($"[green]{Markup.Escape(crewName)}[/]").LeftJustified());
                AnsiConsole.Write(table);
            }
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }

        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task<int> SelectCrewAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllCrewsUseCase(new CrewRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay grupos de tripulación. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el grupo:").PageSize(10)
                .AddChoices(items.Select(c => $"{c.Id.Value}. {c.GroupName.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectEmployeeAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var employees = await new GetAllEmployeesUseCase(new EmployeeRepository(context)).ExecuteAsync(ct);
        var persons = await new GetAllPersonsUseCase(new PersonRepository(context)).ExecuteAsync(ct);
        if (!employees.Any()) throw new InvalidOperationException("No hay empleados registrados. Registra uno primero.");
        var personMap = persons.ToDictionary(p => p.Id.Value, p => $"{p.FirstName.Value} {p.LastName.Value}");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el empleado:").PageSize(10)
                .AddChoices(employees.Select(e =>
                {
                    var name = personMap.TryGetValue(e.IdPerson, out var pn) ? pn : e.IdPerson.ToString();
                    return $"{e.Id.Value}. {name}";
                })));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectRoleAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var items = await new GetAllEmployeeRolesUseCase(new EmployeeRoleRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay roles de empleado. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el rol en la tripulación:").PageSize(10)
                .AddChoices(items.Select(r => $"{r.Id.Value}. {r.Name.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task CreateMemberAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ASIGNAR EMPLEADO A GRUPO[/]").Centered());
        try
        {
            var idCrew = await SelectCrewAsync(ct);
            var idEmployee = await SelectEmployeeAsync(ct);
            var idRole = await SelectRoleAsync(ct);
            using var context = DbContextFactory.Create();
            var result = await new CreateCrewMemberUseCase(new CrewMemberRepository(context))
                .ExecuteAsync(idCrew, idEmployee, idRole, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllCrewMembersUseCase(new CrewMemberRepository(context)).ExecuteAsync(ct))
                .Where(m => m.IdCrew == idCrew && m.IdEmployee == idEmployee && m.IdRole == idRole)
                .OrderByDescending(m => m.Id.Value)
                .Select(m => m.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Empleado asignado al grupo con ID de miembro {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteMemberAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR MIEMBRO DE GRUPO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del miembro a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el miembro con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteCrewMemberUseCase(new CrewMemberRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Miembro eliminado correctamente.[/]" : "\n[yellow]No se encontró el miembro con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
