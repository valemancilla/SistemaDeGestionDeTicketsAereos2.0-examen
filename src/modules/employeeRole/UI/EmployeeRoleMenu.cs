using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.UI;

public sealed class EmployeeRoleMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE ROLES DE EMPLEADO[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear rol", "2. Listar roles", "3. Actualizar rol", "4. Eliminar rol", "0. Volver"));

            switch (option)
            {
                case "1. Crear rol": await CreateAsync(ct); break;
                case "2. Listar roles": await ListAsync(ct); break;
                case "3. Actualizar rol": await UpdateAsync(ct); break;
                case "4. Eliminar rol": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var items = await new GetAllEmployeeRolesUseCase(new EmployeeRoleRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) { AnsiConsole.MarkupLine("[yellow]No hay roles de empleado registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre");
            foreach (var r in items)
                table.AddRow(r.Id.Value.ToString(), Markup.Escape(r.Name.Value));
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR ROL DE EMPLEADO[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un rol de empleado?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre (ej: Piloto, Copiloto, Auxiliar de vuelo):");
        try
        {
            using var context = DbContextFactory.Create();
            var result = await new CreateEmployeeRoleUseCase(new EmployeeRoleRepository(context)).ExecuteAsync(name, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllEmployeeRolesUseCase(new EmployeeRoleRepository(context)).ExecuteAsync(ct))
                .Where(r => r.Name.Value == name)
                .OrderByDescending(r => r.Id.Value)
                .Select(r => r.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Rol '[bold]{Markup.Escape(result.Name.Value)}[/]' creado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR ROL DE EMPLEADO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del rol a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        try
        {
            using var context = DbContextFactory.Create();
            await new UpdateEmployeeRoleUseCase(new EmployeeRoleRepository(context)).ExecuteAsync(id, name, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Rol de empleado actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR ROL DE EMPLEADO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del rol a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el rol con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteEmployeeRoleUseCase(new EmployeeRoleRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Rol eliminado correctamente.[/]" : "\n[yellow]No se encontró el rol con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
