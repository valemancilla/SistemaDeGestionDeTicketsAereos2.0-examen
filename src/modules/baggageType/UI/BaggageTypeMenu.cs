using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Domain;
using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.UI;

public sealed class BaggageTypeMenu
{
    private const string BaggageCarryOnName = "Equipaje de mano";
    private const string BaggageCheckedName = "Equipaje de bodega";
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE TIPOS DE EQUIPAJE[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(7)
                    .AddChoices(
                        "1. Crear tipo",
                        "2. Listar tipos",
                        "3. Actualizar tipo",
                        "4. Activar / desactivar tipo",
                        "5. Eliminar tipo",
                        "6. Política tarifas en cliente (Basic/Classic/Flex)",
                        "0. Volver"));

            switch (option)
            {
                case "1. Crear tipo": await CreateAsync(ct); break;
                case "2. Listar tipos": await ListAsync(ct); break;
                case "3. Actualizar tipo": await UpdateAsync(ct); break;
                case "4. Activar / desactivar tipo": await ToggleActiveAsync(ct); break;
                case "5. Eliminar tipo": await DeleteAsync(ct); break;
                case "6. Política tarifas en cliente (Basic/Classic/Flex)":
                    await RunClientFareBundleDisplayPolicyAsync(ct);
                    break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var items = await new GetAllBaggageTypesUseCase(new BaggageTypeRepository(context)).ExecuteAsync(ct);
        if (!items.Any()) { AnsiConsole.MarkupLine("[yellow]No hay tipos de equipaje registrados.[/]"); }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID");
            table.AddColumn("Nombre");
            table.AddColumn("Peso (kg)");
            table.AddColumn("Precio base (COP)");
            table.AddColumn("Activo");
            foreach (var b in items)
                table.AddRow(
                    b.Id.Value.ToString(),
                    Markup.Escape(b.Name.Value),
                    b.WeightKg.ToString("0.##"),
                    b.BasePriceCop.ToString("0"),
                    b.IsActive ? "Sí" : "No");
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]CREAR TIPO DE EQUIPAJE[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas crear un tipo de equipaje?", true))
            return;
        var name = AnsiConsole.Ask<string>("Nombre (ej: Artículo personal (bolso), Equipaje de mano, Equipaje de bodega):");
        var weight = AnsiConsole.Prompt(new TextPrompt<decimal>("Peso (kg, 0 si no aplica):").DefaultValue(0m));
        var price = AnsiConsole.Prompt(new TextPrompt<decimal>("Precio base (COP, 0 si incluido):").DefaultValue(0m));
        var desc = AnsiConsole.Prompt(new TextPrompt<string>("Descripción (Enter para omitir):").AllowEmpty());
        var active = AnsiConsole.Confirm("¿Activo?", true);
        try
        {
            using var context = DbContextFactory.Create();
            var result = await new CreateBaggageTypeUseCase(new BaggageTypeRepository(context))
                .ExecuteAsync(name, weight, price, string.IsNullOrWhiteSpace(desc) ? null : desc, active, ct);
            await context.SaveChangesAsync(ct);

            var createdId = (await new GetAllBaggageTypesUseCase(new BaggageTypeRepository(context)).ExecuteAsync(ct))
                .Where(b => b.Name.Value == name)
                .OrderByDescending(b => b.Id.Value)
                .Select(b => b.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Tipo '[bold]{Markup.Escape(result.Name.Value)}[/]' creado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR TIPO DE EQUIPAJE[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del tipo a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        try
        {
            using var pre = DbContextFactory.Create();
            var existing = await new GetBaggageTypeByIdUseCase(new BaggageTypeRepository(pre)).ExecuteAsync(id, ct);

            var name = AnsiConsole.Prompt(new TextPrompt<string>("Nombre:").DefaultValue(existing.Name.Value));
            var weight = AnsiConsole.Prompt(new TextPrompt<decimal>("Peso (kg):").DefaultValue(existing.WeightKg));
            var price = AnsiConsole.Prompt(new TextPrompt<decimal>("Precio base (COP):").DefaultValue(existing.BasePriceCop));
            var desc = AnsiConsole.Prompt(new TextPrompt<string>("Descripción (Enter para dejar vacío):").DefaultValue(existing.Description ?? string.Empty));
            var active = AnsiConsole.Confirm("¿Activo?", existing.IsActive);

            using var context = DbContextFactory.Create();
            await new UpdateBaggageTypeUseCase(new BaggageTypeRepository(context))
                .ExecuteAsync(id, name, weight, price, string.IsNullOrWhiteSpace(desc) ? null : desc, active, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Tipo de equipaje actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task ToggleActiveAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTIVAR / DESACTIVAR TIPO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del tipo (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;

        try
        {
            using var pre = DbContextFactory.Create();
            var existing = await new GetBaggageTypeByIdUseCase(new BaggageTypeRepository(pre)).ExecuteAsync(id, ct);
            var next = !existing.IsActive;

            using var context = DbContextFactory.Create();
            await new UpdateBaggageTypeUseCase(new BaggageTypeRepository(context))
                .ExecuteAsync(
                    existing.Id.Value,
                    existing.Name.Value,
                    existing.WeightKg,
                    existing.BasePriceCop,
                    existing.Description,
                    next,
                    ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(next ? "\n[green]Tipo activado.[/]" : "\n[green]Tipo desactivado.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }

        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR TIPO DE EQUIPAJE[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del tipo a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el tipo con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeleteBaggageTypeUseCase(new BaggageTypeRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Tipo eliminado correctamente.[/]" : "\n[yellow]No se encontró el tipo con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    /// <summary>Montos, multiplicadores y textos de las tarjetas de tarifa mostradas al cliente (tabla <c>ClientFareBundleDisplay</c>).</summary>
    private static async Task RunClientFareBundleDisplayPolicyAsync(CancellationToken ct)
    {
        var back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]Política en pantalla cliente (Basic/Classic/Flex)[/]").Centered());
            AnsiConsole.MarkupLine(
                "[grey]Se usa al buscar vuelos. Marcadores en cuerpos y explicación: {{CARRYON}}, {{CHECKED}}, {{SEAT}} (se reemplazan al mostrar). " +
                "Los nombres de tipos de equipaje siguen en «listar tipos» arriba.[/]\n");
            var opt = AnsiConsole.Prompt(
                new SelectionPrompt<string>().PageSize(7).AddChoices(
                    "1. Ver valores actuales",
                    "2. Editar montos, multiplicadores y ref. sin publicar (COP)",
                    "3. Editar subtítulo y texto explicativo",
                    "4. Editar cuerpos de las tarjetas (Basic, Classic, Flex)",
                    "5. Restablecer cuerpos y explicación a valores por defecto",
                    "0. Volver"));

            if (opt == "0. Volver")
            {
                back = true;
                continue;
            }

            try
            {
                using var context = DbContextFactory.Create();
                var repo = new ClientFareBundleDisplayRepository(context);
                var getUc = new GetClientFareBundleDisplayUseCase(repo);
                var updateUc = new UpdateClientFareBundleDisplayUseCase(repo);
                var d = await getUc.ExecuteAsync(ct);

                switch (opt)
                {
                    case "1. Ver valores actuales":
                        ViewClientFareBundleDisplay(d);
                        break;
                    case "2. Editar montos, multiplicadores y ref. sin publicar (COP)":
                        EditClientFareBundleDisplayNumbers(d);
                        await updateUc.ExecuteAsync(d, ct);
                        await context.SaveChangesAsync(ct);
                        AnsiConsole.MarkupLine("\n[green]Cambios guardados.[/]");
                        break;
                    case "3. Editar subtítulo y texto explicativo":
                        d.SubtitleLine = AnsiConsole.Prompt(
                            new TextPrompt<string>("Línea de subtítulo (gris, junto al título):")
                                .DefaultValue(d.SubtitleLine)
                                .ShowDefaultValue());
                        AnsiConsole.MarkupLine(
                            "\n[dim]Texto explicativo: solo Enter en la primera línea = sin cambios; o escribí varias líneas y terminá con una línea vacía (admite negrita y colores Spectre, ej. [bold]Basic[/]).[/]");
                        d.ExplainerLine = ReadMultilineOrKeep(d.ExplainerLine);
                        await updateUc.ExecuteAsync(d, ct);
                        await context.SaveChangesAsync(ct);
                        AnsiConsole.MarkupLine("\n[green]Cambios guardados.[/]");
                        break;
                    case "4. Editar cuerpos de las tarjetas (Basic, Classic, Flex)":
                        AnsiConsole.MarkupLine(
                            "[dim]Para cada cuerpo: Enter en la primera línea = sin cambios; o varias líneas y línea vacía al final.[/]\n");
                        AnsiConsole.MarkupLine("\n[bold]Basic[/]");
                        d.BasicBodyMarkup = ReadMultilineOrKeep(d.BasicBodyMarkup);
                        AnsiConsole.MarkupLine("\n[bold]Classic[/]");
                        d.ClassicBodyMarkup = ReadMultilineOrKeep(d.ClassicBodyMarkup);
                        AnsiConsole.MarkupLine("\n[bold]Flex[/]");
                        d.FlexBodyMarkup = ReadMultilineOrKeep(d.FlexBodyMarkup);
                        await updateUc.ExecuteAsync(d, ct);
                        await context.SaveChangesAsync(ct);
                        AnsiConsole.MarkupLine("\n[green]Cambios guardados.[/]");
                        break;
                    case "5. Restablecer cuerpos y explicación a valores por defecto":
                        if (!AnsiConsole.Confirm(
                                 "¿Sobrescribir subtítulo, explicación y los tres cuerpos con el texto inicial del sistema?"))
                            break;
                        d.SubtitleLine = ClientFareBundleDisplayDefaults.SubtitleLine;
                        d.ExplainerLine = ClientFareBundleDisplayDefaults.ExplainerLineTemplate;
                        d.BasicBodyMarkup = ClientFareBundleDisplayDefaults.BasicBody();
                        d.ClassicBodyMarkup = ClientFareBundleDisplayDefaults.ClassicBody;
                        d.FlexBodyMarkup = ClientFareBundleDisplayDefaults.FlexBody;
                        await updateUc.ExecuteAsync(d, ct);
                        await context.SaveChangesAsync(ct);
                        AnsiConsole.MarkupLine(
                            "\n[green]Textos y subtítulo restablecidos. Los montos y multiplicadores no cambiaron.[/]");
                        break;
                }
            }
            catch (Exception ex)
            {
                EntityPersistenceUiFeedback.Write(ex);
            }

            if (!back)
            {
                ConsolaPausa.PresionarCualquierTecla();
            }
        }
    }

    private static void ViewClientFareBundleDisplay(ClientFareBundleDisplayData d)
    {
        using var ctx = DbContextFactory.Create();
        var btypes = new GetAllBaggageTypesUseCase(new BaggageTypeRepository(ctx)).ExecuteAsync().GetAwaiter().GetResult();
        var carry = btypes.FirstOrDefault(x => string.Equals(x.Name.Value, BaggageCarryOnName, StringComparison.OrdinalIgnoreCase));
        var checkedB = btypes.FirstOrDefault(x => string.Equals(x.Name.Value, BaggageCheckedName, StringComparison.OrdinalIgnoreCase));
        var refCarry = carry?.BasePriceCop ?? d.RefCarryOnCop;
        var refChecked = checkedB?.BasePriceCop ?? d.RefCheckedCop;

        var t = new Table().Border(TableBorder.Rounded);
        t.AddColumn("Campo");
        t.AddColumn("Valor");
        t.AddRow("Ref. mano (COP)", Markup.Escape(ClientFareBundleDisplayDefaults.FormatPriceCopColombia(refCarry)));
        t.AddRow("Ref. bodega (COP)", Markup.Escape(ClientFareBundleDisplayDefaults.FormatPriceCopColombia(refChecked)));
        t.AddRow("Multiplicador Classic", d.ClassicMultiplier.ToString(System.Globalization.CultureInfo.InvariantCulture));
        t.AddRow("Multiplicador Flex", d.FlexMultiplier.ToString(System.Globalization.CultureInfo.InvariantCulture));
        t.AddRow("Ref. pasaje sin publicar (COP)", Markup.Escape(ClientFareBundleDisplayDefaults.FormatPriceCopColombia(d.UnpublishedFareReferenceCop)));
        t.AddRow("Asiento desde (COP)", Markup.Escape(ClientFareBundleDisplayDefaults.FormatPriceCopColombia(d.SeatSelectionFromCop)));
        AnsiConsole.Write(t);
        AnsiConsole.MarkupLine($"\n[bold]Subtítulo[/] [grey]{Markup.Escape(d.SubtitleLine)}[/]");
        var expl = ClientFareBundleDisplayDefaults.ApplyPricePlaceholders(
            d.ExplainerLine, refCarry, refChecked, d.SeatSelectionFromCop);
        AnsiConsole.MarkupLine($"\n[bold]Explicación (vista con precios resueltos)[/]\n[grey]{Markup.Escape(StripMarkupForAdminPreview(expl))}[/]");
        var basicPrev = ClientFareBundleDisplayDefaults.ApplyPricePlaceholders(
            d.BasicBodyMarkup, refCarry, refChecked, d.SeatSelectionFromCop);
        AnsiConsole.MarkupLine($"\n[bold]Basic (preview sin colores, solo texto / marcadores resueltos)[/]\n[dim]{Markup.Escape(TruncateForPreview(basicPrev, 400))}[/]");
        var classicPrev = ClientFareBundleDisplayDefaults.ApplyPricePlaceholders(
            d.ClassicBodyMarkup, refCarry, refChecked, d.SeatSelectionFromCop);
        AnsiConsole.MarkupLine($"\n[bold]Classic[/]\n[dim]{Markup.Escape(TruncateForPreview(classicPrev, 400))}[/]");
        var flexPrev = ClientFareBundleDisplayDefaults.ApplyPricePlaceholders(
            d.FlexBodyMarkup, refCarry, refChecked, d.SeatSelectionFromCop);
        AnsiConsole.MarkupLine($"\n[bold]Flex[/]\n[dim]{Markup.Escape(TruncateForPreview(flexPrev, 400))}[/]");
    }

    private static string StripMarkupForAdminPreview(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return string.Empty;
        // Vista admin legible: quitar tags [estilo] del markup Spectre; no afecta lo guardado.
        return System.Text.RegularExpressions.Regex.Replace(raw, @"\[[^\]]*\]", string.Empty, System.Text.RegularExpressions.RegexOptions.None);
    }

    private static string TruncateForPreview(string? s, int max)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        s = s.Replace('\n', ' ');
        return s.Length <= max ? s : s[..max] + "…";
    }

    private static void EditClientFareBundleDisplayNumbers(ClientFareBundleDisplayData d)
    {
        AnsiConsole.MarkupLine(
            "[grey]Los valores de referencia de equipaje de mano y bodega se gestionan en «Tipos de equipaje» (precio base).[/]\n");
        d.ClassicMultiplier = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Multiplicador respecto a Basic para Classic (ej. 1,465):")
                .DefaultValue(d.ClassicMultiplier)
                .ShowDefaultValue());
        d.FlexMultiplier = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Multiplicador respecto a Basic para Flex (ej. 1,6285):")
                .DefaultValue(d.FlexMultiplier)
                .ShowDefaultValue());
        d.UnpublishedFareReferenceCop = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Ref. de pasaje si no hay tarifa publicada (COP):")
                .DefaultValue(d.UnpublishedFareReferenceCop)
                .ShowDefaultValue());
        d.SeatSelectionFromCop = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Precio de referencia «asiento desde» (COP):")
                .DefaultValue(d.SeatSelectionFromCop)
                .ShowDefaultValue());
    }

    /// <summary>Primera línea vacía = mantener; si no, varias líneas hasta una línea vacía.</summary>
    private static string ReadMultilineOrKeep(string? current)
    {
        var first = Console.ReadLine();
        if (string.IsNullOrEmpty(first) && !string.IsNullOrEmpty(current))
            return current;
        if (string.IsNullOrEmpty(first))
            return string.Empty;
        var lines = new List<string> { first };
        while (true)
        {
            var line = Console.ReadLine();
            if (line is null) break;
            if (line.Length == 0) break;
            lines.Add(line);
        }

        return string.Join("\n", lines);
    }
}
