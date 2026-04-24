using System.Globalization;

namespace SistemaDeGestionDeTicketsAereos.src.shared.helpers;

/// <summary>Textos y valores por defecto de las tarjetas Basic/Classic/Flex (pantalla cliente; persistidos en <c>ClientFareBundleDisplay</c>).</summary>
public static class ClientFareBundleDisplayDefaults
{
    public const decimal ReferenceCarryOnCop = 70_000m;
    public const decimal ReferenceCheckedCop = 70_000m;
    public const decimal ClassicMultiplier = 1.465m;
    public const decimal FlexMultiplier = 1.6285m;
    public const decimal UnpublishedFareReferenceCop = 250_000m;

    public const string SubtitleLine = "(referencia por pasajero; tipos de equipaje los gestiona el administrador)";

    // Marcadores CARRYON / CHECKED reemplazados con FormatPriceCopColombia
    public const string ExplainerLineTemplate =
        "En [bold]Basic[/] el total de referencia suma al pasaje el equipaje de mano y de bodega ({{CARRYON}} + {{CHECKED}}). " +
        "En Classic y Flex ese equipaje va incluido en la tarifa mostrada en las tarjetas.";

    public static string FormatPriceCopColombia(decimal amount)
    {
        var n = amount.ToString("N0", CultureInfo.GetCultureInfo("es-CO"))
            .Replace('\u00a0', ' ')
            .Trim();
        return "$" + n + " COP";
    }

    public static string ApplyPricePlaceholders(string? markup, decimal carry, decimal check, decimal seat)
    {
        if (string.IsNullOrEmpty(markup)) return string.Empty;
        return markup
            .Replace("{{CARRYON}}", FormatPriceCopColombia(carry))
            .Replace("{{CHECKED}}", FormatPriceCopColombia(check))
            .Replace("{{SEAT}}", FormatPriceCopColombia(seat));
    }

    public static string BasicBody()
    {
        // Marcadores reemplazados al mostrar
        return """
            [bold]Incluye[/]
            [#db2777]✓[/] 1 artículo personal (bolso)
            [#db2777]✓[/] Acumula 3 millas por USD
            [grey]$ Equipaje de mano (10 kg) - Desde {{CARRYON}}[/]
            [grey]$ Equipaje de bodega (23 kg) - Desde {{CHECKED}}[/]
            [grey]$ Check-in en aeropuerto[/]
            [grey]$ Selección de asientos - Desde {{SEAT}}[/]
            [grey]$ Menú a bordo[/]
            [grey]$ Cambios antes del vuelo[/]
            [grey]✗ Reembolsos antes del vuelo[/]
            """;
    }

    public const string ClassicBody = """
[bold]Incluye[/]
[#6d28d9]✓[/] 1 artículo personal (bolso)
[#6d28d9]✓[/] 1 equipaje de mano (10 kg)
[#6d28d9]✓[/] 1 equipaje de bodega (23 kg)
[#6d28d9]✓[/] Check-in en aeropuerto
[#6d28d9]✓[/] Asiento Economy incluido
[#6d28d9]✓[/] Acumula 6 millas por USD
[grey]$ Menú a bordo[/]
[grey]$ Cambios antes del vuelo[/]
[grey]✗ Reembolsos antes del vuelo[/]
""";

    public const string FlexBody = """
[bold]Incluye[/]
[#ea580c]✓[/] 1 artículo personal (bolso)
[#ea580c]✓[/] 1 equipaje de mano (10 kg)
[#ea580c]✓[/] 1 equipaje de bodega (23 kg)
[#ea580c]✓[/] Check-in en aeropuerto
[#ea580c]✓[/] Asiento Plus
[#ea580c]✓[/] Acumula 8 millas por USD
[#ea580c]✓[/] Cambios antes del vuelo
[#ea580c]✓[/] Reembolsos antes del vuelo
[grey]$ Menú a bordo[/]
""";

    public const decimal SeatSelectionFromCop = 27_000m;
}
