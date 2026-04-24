using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.UI;

public sealed class PaymentMethodMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE MÉTODOS DE PAGO[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices("1. Crear método", "2. Listar métodos", "3. Actualizar método", "4. Eliminar método", "0. Volver"));

            switch (option)
            {
                case "1. Crear método": await CreateAsync(ct); break;
                case "2. Listar métodos": await ListAsync(ct); break;
                case "3. Actualizar método": await UpdateAsync(ct); break;
                case "4. Eliminar método": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var items = await new GetAllPaymentMethodsUseCase(new PaymentMethodRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) { AnsiConsole.MarkupLine("[yellow]No hay métodos de pago registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Nombre");
            foreach (var p in items)
                table.AddRow(p.Id.Value.ToString(), Markup.Escape(p.Name.Value));
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR MÉTODO DE PAGO[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un método de pago?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre (ej: Tarjeta de crédito, Efectivo, PSE):");
        try
        {
            using var context = DbContextFactory.Create();
            var result = await new CreatePaymentMethodUseCase(new PaymentMethodRepository(context)).ExecuteAsync(name, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllPaymentMethodsUseCase(new PaymentMethodRepository(context)).ExecuteAsync(ct))
                .Where(p => p.Name.Value == name)
                .OrderByDescending(p => p.Id.Value)
                .Select(p => p.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Método '[bold]{Markup.Escape(result.Name.Value)}[/]' creado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR MÉTODO DE PAGO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del método a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var name = AnsiConsole.Ask<string>("Nuevo nombre:");
        try
        {
            using var context = DbContextFactory.Create();
            await new UpdatePaymentMethodUseCase(new PaymentMethodRepository(context)).ExecuteAsync(id, name, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Método de pago actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR MÉTODO DE PAGO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del método a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el método con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeletePaymentMethodUseCase(new PaymentMethodRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Método eliminado correctamente.[/]" : "\n[yellow]No se encontró el método con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
