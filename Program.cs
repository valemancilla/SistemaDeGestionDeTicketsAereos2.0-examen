using System.IO;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;

try
{
    // Verificamos conexión en segundo plano (opcional) o la omitimos del output visual
    using var context = DbContextFactory.Create();

    // Lanzamos el menú visual interactivo
    await ConsoleMenuOrchestrator.StartAsync();
}
catch (Exception ex)
{
    try { Console.Clear(); }
    catch (IOException) { /* consola no interactiva o controlador no válido */ }

    Console.Error.WriteLine($"Error crítico que detuvo el programa: {ex.Message}");
    if (ex.InnerException != null)
    {
         Console.Error.WriteLine($"Detalle: {ex.InnerException.Message}");
    }
}
