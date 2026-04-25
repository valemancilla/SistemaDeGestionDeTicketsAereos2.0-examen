// =============================================================================
// Punto de entrada de la aplicación de consola.
// - Comprueba que el terminal sea interactivo (Spectre.Console).
// - Inicia el bucle de menús (login → administrador o cliente) vía ConsoleMenuOrchestrator.
// =============================================================================
using System.IO;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;
using Spectre.Console;

try
{
    using var context = DbContextFactory.Create();
    if (!AnsiConsole.Profile.Capabilities.Interactive)
    {
        var cwd = Directory.GetCurrentDirectory();
        await Console.Error.WriteLineAsync();
        await Console.Error.WriteLineAsync("Este programa usa menús interactivos (teclado) con Spectre.Console.");
        await Console.Error.WriteLineAsync("El terminal actual no informa consola interactiva (pasa al ejecutar desde el panel");
        await Console.Error.WriteLineAsync("«Run» de algunos editores, o con entrada/salida redirigida).");
        await Console.Error.WriteLineAsync();
        await Console.Error.WriteLineAsync("Solución: abrí Windows Terminal, PowerShell o CMD y ejecutá:");
        await Console.Error.WriteLineAsync($"  cd \"{cwd}\"");
        await Console.Error.WriteLineAsync("  dotnet run");
        await Console.Error.WriteLineAsync();
        return;
    }

    await ConsoleMenuOrchestrator.StartAsync();
}
catch (Exception ex) when (ex is NotSupportedException n && n.Message.Contains("interactive", StringComparison.OrdinalIgnoreCase))
{
    await WriteInteractiveConsoleHelpAsync();
}
catch (Exception ex)
{
    try { Console.Clear(); }
    catch { /* consola no interactiva o controlador no válido */ }

    Console.Error.WriteLine("Error crítico que detuvo el programa:");
    Console.Error.WriteLine(ex.ToString());
}

static async Task WriteInteractiveConsoleHelpAsync()
{
    var cwd = Directory.GetCurrentDirectory();
    await Console.Error.WriteLineAsync();
    await Console.Error.WriteLineAsync("Consola no interactiva: no se pueden mostrar los menús.");
    await Console.Error.WriteLineAsync("Abrí PowerShell/Terminal de Windows, cd a la carpeta del proyecto y: dotnet run");
    await Console.Error.WriteLineAsync($"Carpeta del proyecto: {cwd}");
    await Console.Error.WriteLineAsync();
}
