using SistemaDeGestionDeTicketsAereos.src.modules.role.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.UI;

public sealed class UserMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE USUARIOS DEL SISTEMA[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear usuario", "2. Listar usuarios", "3. Actualizar usuario", "4. Eliminar usuario", "0. Volver"));

            switch (option)
            {
                case "1. Crear usuario": await CreateAsync(ct); break;
                case "2. Listar usuarios": await ListAsync(ct); break;
                case "3. Actualizar usuario": await UpdateAsync(ct); break;
                case "4. Eliminar usuario": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var items = await new GetAllUsersUseCase(new UserRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) { AnsiConsole.MarkupLine("[yellow]No hay usuarios registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Username"); table.AddColumn("Rol"); table.AddColumn("Activo");
            foreach (var u in items)
                table.AddRow(u.Id.Value.ToString(), Markup.Escape(u.Username.Value),
                    u.IdUserRole.ToString(), u.Active ? "[green]Sí[/]" : "[red]No[/]");
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task<int> SelectRoleAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var roles = await new GetAllRolesUseCase(new RoleRepository(context)).ExecuteAsync(ct);
        if (!roles.Any()) throw new InvalidOperationException("No hay roles registrados. Crea un rol primero.");
        var choices = roles.Select(r => $"{r.Id.Value}. {r.Name.Value}").ToList();
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el rol:").AddChoices(choices));
        return int.Parse(selected.Split('.')[0]);
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR USUARIO[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un usuario?", true))
            return;
        var username = AnsiConsole.Ask<string>("Username:");
        var password = AnsiConsole.Prompt(new TextPrompt<string>("Contraseña:").Secret());
        var confirm = AnsiConsole.Prompt(new TextPrompt<string>("Confirmar contraseña:").Secret());
        if (password != confirm)
        {
            AnsiConsole.MarkupLine("\n[red]Las contraseñas no coinciden.[/]");
            Console.ReadKey(); return;
        }
        try
        {
            var idRole = await SelectRoleAsync(ct);
            var active = AnsiConsole.Confirm("¿Usuario activo?", true);
            using var context = DbContextFactory.Create();
            var result = await new CreateUserUseCase(new UserRepository(context)).ExecuteAsync(username, password, idRole, null, active, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllUsersUseCase(new UserRepository(context)).ExecuteAsync(ct))
                .Where(u => u.Username.Value == username && u.IdUserRole == idRole && u.Active == active)
                .OrderByDescending(u => u.Id.Value)
                .Select(u => u.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Usuario '[bold]{Markup.Escape(result.Username.Value)}[/]' creado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR USUARIO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del usuario a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var username = AnsiConsole.Ask<string>("Nuevo username:");
        var password = AnsiConsole.Prompt(new TextPrompt<string>("Nueva contraseña:").Secret());
        try
        {
            var idRole = await SelectRoleAsync(ct);
            var active = AnsiConsole.Confirm("¿Usuario activo?", true);
            using var context = DbContextFactory.Create();
            await new UpdateUserUseCase(new UserRepository(context)).ExecuteAsync(id, username, password, idRole, null, active, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Usuario actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR USUARIO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del usuario a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el usuario con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteUserUseCase(new UserRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Usuario eliminado correctamente.[/]" : "\n[yellow]No se encontró el usuario con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
