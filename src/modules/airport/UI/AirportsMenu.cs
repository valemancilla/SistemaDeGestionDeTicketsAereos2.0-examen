using SistemaDeGestionDeTicketsAereos.src.modules.route.UI;
using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using Spectre.Console;

using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.UI;

public class AirportsMenu : IModuleUI
{
    public string Key => "2";
    public string Title => "Gestión de Aeropuertos y Rutas";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule($"[green]{Key}. {Title.ToUpper()}[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(5)
                    .AddChoices(
                        "1. Aeropuertos",
                        "2. Rutas",
                        "0. Volver"));

            try
            {
                switch (option)
                {
                    case "1. Aeropuertos": await new AirportMenu().RunAsync(cancellationToken); break;
                    case "2. Rutas":       await new RouteMenu().RunAsync(cancellationToken);   break;
                    case "0. Volver": back = true; break;
                }
            }
            catch (Exception ex)
            {
                EntityPersistenceUiFeedback.Write(ex);
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
                Console.ReadKey();
            }
        }
    }
}
