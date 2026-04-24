using SistemaDeGestionDeTicketsAereos.src.modules.admin.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.report.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.UI;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;

public static class ConsoleMenuOrchestrator
{
    public static async Task StartAsync()
    {
        // Interceptamos Ctrl+C para evitar cierres abruptos. 
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; 
        };

    while (true) // Bucle infinito: mantiene la aplicación corriendo siempre
    {
    // Si el usuario NO está autenticado (no ha iniciado sesión)
        if (!AppState.IsAuthenticated)
        {
        // Muestra el menú de login y espera a que el usuario inicie sesión
            await new LoginMenu().RunAsync();
        }
        else
        {
        // Variable que indica si se debe cerrar la aplicación
            bool exitApp = false;
        
        //  Control de acceso basado en el rol del usuario
            if (AppState.IdUserRole == 1) // Administrador
            {
            // Muestra el menú principal de administrador
            // Este método devuelve true si el usuario decide salir
                exitApp = await ShowAdminMainMenuAsync();
            }
            else // Cliente u otros roles 
            {
            // Muestra el menú principal de cliente
                exitApp = await ShowCustomerMainMenuAsync();
            }

        // Si el usuario eligió salir, se rompe el bucle infinito
        if (exitApp) break;
        }
    }
    }

   /// <summary>
/// Muestra el menú principal del administrador.
/// Retorna true si el usuario decide salir de la aplicación.
/// </summary>
private static async Task<bool> ShowAdminMainMenuAsync()
{
    // Variable que controla si se debe salir del menú
    bool exit = false;

    // Bucle que mantiene el menú ejecutándose hasta que el usuario salga
    while (!exit)
    {
        // Limpia la consola para mostrar el menú "fresco"
        Console.Clear();

        // Título principal del sistema (con estilo)
        AnsiConsole.Write(new Rule("[yellow]SISTEMA DE GESTIÓN DE TIQUETES AÉREOS[/]").Centered());

        // Subtítulo indicando que es el menú del administrador
        AnsiConsole.Write(new Rule("[red]MENÚ PRINCIPAL (ADMINISTRADOR)[/]").Centered());

        // Muestra un menú interactivo donde el usuario selecciona con flechas
        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Seleccione una opción utilizando las flechas (Arriba/Abajo) y presione Enter:")
                .PageSize(12) // Cantidad de opciones visibles antes de hacer scroll
                .HighlightStyle(new Style(foreground: Color.Red, background: Color.Black)) // Estilo de la opción seleccionada
                .AddChoices(new[] {
                    // Orden sugerido de creación/configuración:
                    // 1) Catálogos base → 2) Aerolíneas/Aeronaves → 3) Aeropuertos/Rutas → 4) Tripulación → 5) Vuelos/Tarifas → 6+) Operación
                    "1. Administración (Catálogos)",
                    "2. Gestión de Aerolíneas y Aeronaves",
                    "3. Gestión de Aeropuertos y Rutas",
                    "4. Gestión de Tripulación",
                    "5. Gestión de Vuelos y Tarifas",
                    "6. Gestión de Clientes",
                    "7. Gestión de Reservas",
                    "8. Gestión de Tiquetes y Check-In",
                    "9. Gestión de Pagos",
                    "10. Reportes LINQ",
                    "0. Salir"
                }));

        // Evalúa la opción que eligió el usuario
        switch (option)
        {
            case "1. Administración (Catálogos)":
                await new AdminMenu().RunAsync();
                break;

            case "2. Gestión de Aerolíneas y Aeronaves":
                await new AirlinesMenu().RunAsync();
                break;

            case "3. Gestión de Aeropuertos y Rutas":
                await new AirportsMenu().RunAsync();
                break;

            case "4. Gestión de Tripulación":
                await new CrewMenu().RunAsync();
                break;

            case "5. Gestión de Vuelos y Tarifas":
                await new FlightsMenu().RunAsync();
                break;

            case "6. Gestión de Clientes":
                await new CustomersMenu().RunAsync();
                break;

            case "7. Gestión de Reservas":
                await new BookingsMenu().RunAsync();
                break;

            case "8. Gestión de Tiquetes y Check-In":
                await new TicketsMenu().RunAsync();
                break;

            case "9. Gestión de Pagos":
                await new PaymentsMenu().RunAsync();
                break;

            case "10. Reportes LINQ":
                await new ReportsMenu().RunAsync();
                break;

            case "0. Salir":
                // Si el usuario elige salir, retorna true
                // Esto hará que el programa principal termine
                return true;
        }
    }

    // Este return casi nunca se alcanza, pero es obligatorio por el tipo Task<bool>
    return false;
}

    /// <summary>
    /// Muestra únicamente las funciones de autogestión. Exclusivo para Clientes finales.
    /// </summary>
    private static async Task<bool> ShowCustomerMainMenuAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]TIQUETES AÉREOS — CLIENTE[/]").Centered());
            var name = string.IsNullOrWhiteSpace(AppState.CurrentUser)
                ? "cliente"
                : AppState.CurrentUser.Trim().ToUpperInvariant();
            AnsiConsole.MarkupLine($"[green]Hola, {Markup.Escape(name)}[/]\n");
            AnsiConsole.MarkupLine(
                "[grey]Lo habitual: entrá a [bold]Buscar vuelos[/] y seguí los pasos (vuelo → asientos → pasajeros → pago).[/]");
            AnsiConsole.MarkupLine(
                "[grey][bold]Mis reservas y pagos[/]: ver solo reservas [bold]pagadas[/], ver pagos o cancelar; el pago de una reserva nueva va en [bold]Buscar vuelos[/]. [bold]Check-in[/] cuando ya tengas tiquete.[/]\n");

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]¿Qué deseas hacer?[/]")
                    .PageSize(8)
                    .HighlightStyle(new Style(foreground: Color.Green, background: Color.Black))
                    .AddChoices(new[] {
                        "1. Buscar vuelos (reservar)",
                        "2. Mis reservas y pagos",
                        "3. Check-in y tiquetes",
                        "4. Mi perfil",
                        "0. Salir"
                    }));

            switch (option)
            {
                case "1. Buscar vuelos (reservar)":
                    await new FlightsMenu().RunAsync();
                    break;
                case "2. Mis reservas y pagos":
                    await new BookingsMenu().RunAsync();
                    break;
                case "3. Check-in y tiquetes":
                    await new TicketsMenu().RunAsync();
                    break;
                case "4. Mi perfil":
                    await new CustomersMenu().RunAsync();
                    break;
                case "0. Salir":
                    return true;
            }
        }
        return false;
    }
}
