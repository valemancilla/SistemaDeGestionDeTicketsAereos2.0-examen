using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.shared.helpers;

/// <summary>
/// Unifica el patrón “mensaje gris + ReadKey” repetido en menús de consola
/// (mismo texto y mismo comportamiento que el código en línea anterior).
/// </summary>
public static class ConsolaPausa
{
    /// <param name="conLineaInicial">
    /// <see langword="true"/>: equivalente a <c>MarkupLine("\n[grey]…")</c>;
    /// <see langword="false"/>: equivalente a <c>MarkupLine("[grey]…")</c> sin <c>\n</c> inicial.
    /// </param>
    /// <param name="ocultarTeclaPulsada">Se pasa a <see cref="Console.ReadKey(bool)"/>.</param>
    public static void PresionarCualquierTecla(bool conLineaInicial = true, bool ocultarTeclaPulsada = false)
    {
        if (conLineaInicial)
            AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]");
        else
            AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey(ocultarTeclaPulsada);
    }
}
