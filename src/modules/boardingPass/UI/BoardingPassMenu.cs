using System.Globalization;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.UI;

public sealed class BoardingPassMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        var back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]PASES DE ABORDAR[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(8)
                    .AddChoices(
                        "1. Listar pases",
                        "2. Consultar por id",
                        "3. Consultar por código",
                        "4. Crear pase",
                        "5. Actualizar pase",
                        "6. Eliminar pase",
                        "0. Volver"));

            switch (option)
            {
                case "1. Listar pases": await ListAsync(ct); break;
                case "2. Consultar por id": await GetByIdAsync(ct); break;
                case "3. Consultar por código": await GetByCodeAsync(ct); break;
                case "4. Crear pase": await CreateAsync(ct); break;
                case "5. Actualizar pase": await UpdateAsync(ct); break;
                case "6. Eliminar pase": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var list = await new GetAllBoardingPassesUseCase(new BoardingPassRepository(context)).ExecuteAsync(ct);
        if (list.Count == 0)
            AnsiConsole.MarkupLine("[yellow]No hay pases de abordar.[/]");
        else
        {
            var t = new Table().Border(TableBorder.Rounded);
            t.AddColumn("Id");
            t.AddColumn("Código");
            t.AddColumn("Id tiquete");
            t.AddColumn("Id asiento");
            t.AddColumn("Puerta");
            t.AddColumn("Abordaje");
            t.AddColumn("Id estado");
            foreach (var p in list)
            {
                t.AddRow(
                    p.Id.Value.ToString(),
                    Markup.Escape(p.Code.Value),
                    p.IdTicket.ToString(),
                    p.IdSeat.ToString(),
                    Markup.Escape(p.Gate.Value),
                    p.BoardingTime.ToString("yyyy-MM-dd HH:mm"),
                    p.IdStatus.ToString());
            }
            AnsiConsole.Write(t);
        }

        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla...[/]");
        Console.ReadKey();
    }

    private static async Task GetByIdAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]PASE POR ID[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("Id del pase (0 = volver):")
                .DefaultValue(0)
                .Validate(n => n >= 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]El id no puede ser negativo.[/]")));
        if (id == 0) return;
        try
        {
            using var context = DbContextFactory.Create();
            var p = await new GetBoardingPassByIdUseCase(new BoardingPassRepository(context)).ExecuteAsync(id, ct);
            AnsiConsole.Write(DetailPanel(p));
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]"); Console.ReadKey();
    }

    private static async Task GetByCodeAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]PASE POR CÓDIGO[/]").Centered());
        var code = AnsiConsole.Ask<string>("Código del pase:");
        if (string.IsNullOrWhiteSpace(code)) return;
        using var context = DbContextFactory.Create();
        var p = await new GetBoardingPassByCodeUseCase(new BoardingPassRepository(context)).ExecuteAsync(code, ct);
        if (p is null) AnsiConsole.MarkupLine("[yellow]No se encontró el pase.[/]");
        else AnsiConsole.Write(DetailPanel(p));
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]"); Console.ReadKey();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR PASE DE ABORDAR[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas registrar un pase?", true)) return;
        var code = AnsiConsole.Ask<string>("Código (6-25 caracteres):");
        var idTicket = AnsiConsole.Prompt(new TextPrompt<int>("Id tiquete:").Validate(i => i > 0
            ? ValidationResult.Success()
            : ValidationResult.Error("[red]Debe ser mayor a 0[/]")));
        var idSeat = AnsiConsole.Prompt(new TextPrompt<int>("Id asiento:").Validate(i => i > 0
            ? ValidationResult.Success()
            : ValidationResult.Error("[red]Debe ser mayor a 0[/]")));
        var gate = AnsiConsole.Ask<string>("Puerta (ej. A12):");
        var boardingS = AnsiConsole.Prompt(
            new TextPrompt<string>("Hora de abordaje (yyyy-MM-dd HH:mm):")
                .DefaultValue(DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
        if (!DateTime.TryParse(boardingS, out var boardingTime))
        {
            AnsiConsole.MarkupLine("[red]Fecha/hora no válida.[/]"); Console.ReadKey(); return;
        }
        var idStatus = AnsiConsole.Prompt(
            new TextPrompt<int>("[grey]Id estado[/] (23=Generado, 24=Activo):")
                .DefaultValue(23));
        var pax = AnsiConsole.Ask<string>("Nombre y apellido del pasajero (constante en el pase, opcional):");
        pax = (pax ?? string.Empty).Trim();
        if (pax.Length > 200) pax = pax[..200];
        var createdAt = DateTime.UtcNow;
        try
        {
            using var context = DbContextFactory.Create();
            var repo = new BoardingPassRepository(context);
            var created = await new CreateBoardingPassUseCase(repo)
                .ExecuteAsync(code, idTicket, idSeat, gate, boardingTime, createdAt, idStatus, pax, ct);
            await context.SaveChangesAsync(ct);
            var reloaded = await new GetBoardingPassByCodeUseCase(repo).ExecuteAsync(created.Code.Value, ct);
            var newId = reloaded?.Id.Value ?? 0;
            AnsiConsole.MarkupLine($"\n[green]Pase creado. Id: {newId}. Código: {Markup.Escape(created.Code.Value)}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]"); Console.ReadKey();
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR PASE[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("Id del pase a actualizar (0 = volver):")
                .DefaultValue(0)
                .Validate(n => n >= 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]El id no puede ser negativo.[/]")));
        if (id == 0) return;
        try
        {
            using var c0 = DbContextFactory.Create();
            var exist = await new GetBoardingPassByIdUseCase(new BoardingPassRepository(c0)).ExecuteAsync(id, ct);
            var code = AnsiConsole.Prompt(
                new TextPrompt<string>("Código:")
                    .DefaultValue(exist.Code.Value));
            var idTicket = AnsiConsole.Prompt(
                new TextPrompt<int>("Id tiquete:")
                    .DefaultValue(exist.IdTicket)
                    .Validate(i => i > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Debe ser mayor a 0[/]")));
            var idSeat = AnsiConsole.Prompt(
                new TextPrompt<int>("Id asiento:")
                    .DefaultValue(exist.IdSeat)
                    .Validate(i => i > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Debe ser mayor a 0[/]")));
            var gate = AnsiConsole.Prompt(
                new TextPrompt<string>("Puerta:")
                    .DefaultValue(exist.Gate.Value));
            var boardingS = AnsiConsole.Prompt(
                new TextPrompt<string>("Hora de abordaje (yyyy-MM-dd HH:mm):")
                    .DefaultValue(exist.BoardingTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)));
            if (!DateTime.TryParse(boardingS, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var boardingTime))
            {
                AnsiConsole.MarkupLine("[red]Fecha no válida.[/]"); Console.ReadKey(); return;
            }
            var idStatus = AnsiConsole.Prompt(
                new TextPrompt<int>("Id estado:")
                    .DefaultValue(exist.IdStatus));
            var createdAtS = AnsiConsole.Prompt(
                new TextPrompt<string>("Creado en (yyyy-MM-dd HH:mm):")
                    .DefaultValue(exist.CreatedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)));
            if (!DateTime.TryParse(createdAtS, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var created))
            {
                AnsiConsole.MarkupLine("[red]Fecha creado no válida.[/]"); Console.ReadKey(); return;
            }
            var pax = AnsiConsole.Prompt(
                new TextPrompt<string>("Nombre y apellido (pasajero, literal en pase):")
                    .DefaultValue(exist.PassengerFullName));
            pax = (pax ?? string.Empty).Trim();
            if (pax.Length > 200) pax = pax[..200];

            using var context = DbContextFactory.Create();
            await new UpdateBoardingPassUseCase(new BoardingPassRepository(context))
                .ExecuteAsync(id, code, idTicket, idSeat, gate, boardingTime, created, idStatus, pax, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Pase actualizado.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]"); Console.ReadKey();
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR PASE[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("Id a eliminar (0 = volver):")
                .DefaultValue(0)
                .Validate(n => n >= 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]El id no puede ser negativo.[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Eliminar pase {id}?", true)) { AnsiConsole.MarkupLine("[grey]Cancelado.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var ok = await new DeleteBoardingPassUseCase(new BoardingPassRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(ok ? "\n[green]Eliminado.[/]" : "\n[yellow]No existía ese id.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla...[/]"); Console.ReadKey();
    }

    private static Panel DetailPanel(BoardingPass p) =>
        new Panel($"[bold]Id:[/] {p.Id.Value}\n" +
             $"[bold]Código:[/] {Markup.Escape(p.Code.Value)}\n" +
             $"[bold]Pasajero:[/] {Markup.Escape(p.PassengerFullName)}\n" +
             $"[bold]Id tiquete:[/] {p.IdTicket}\n" +
             $"[bold]Id asiento:[/] {p.IdSeat}\n" +
             $"[bold]Puerta:[/] {Markup.Escape(p.Gate.Value)}\n" +
             $"[bold]Hora de abordaje:[/] {p.BoardingTime:yyyy-MM-dd HH:mm}\n" +
             $"[bold]Registro creado:[/] {p.CreatedAt:yyyy-MM-dd HH:mm}\n" +
             $"[bold]Id estado:[/] {p.IdStatus}")
            .Header("[green]PASE DE ABORDAR[/]")
            .Border(BoxBorder.Rounded);
}
