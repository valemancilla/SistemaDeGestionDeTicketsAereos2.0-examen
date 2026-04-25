using SistemaDeGestionDeTicketsAereos.src.modules.fare.UI;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.UI;

public class FlightsMenu : IModuleUI
{
    public string Key => "3";
    public string Title => "Gestión de Vuelos";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool back = false;
        bool isAdmin = AppState.IdUserRole == 1;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule($"[green]{Key}. {Title.ToUpper()}[/]").Centered());

            if (isAdmin)
            {
                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(5)
                        .AddChoices("1. Tarifas", "2. Vuelos", "0. Volver"));
                try
                {
                    switch (option)
                    {
                        case "1. Tarifas": await new FareMenu().RunAsync(cancellationToken); break;
                        case "2. Vuelos": await new FlightMenu().RunAsync(cancellationToken); break;
                        case "0. Volver": back = true; break;
                    }
                }
                catch (Exception ex)
                {
                    EntityPersistenceUiFeedback.Write(ex);
                    ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
                }
            }
            else
            {
                var clientDone = false;
                while (!clientDone)
                {
                    try
                    {
                        await new FlightMenu().ShowAvailableFlightsAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        EntityPersistenceUiFeedback.Write(ex);
                        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
                    }

                    var next = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("\nVuelos (cliente)")
                            .PageSize(4)
                            .AddChoices("1. Buscar vuelos otra vez", "0. Volver al menú anterior"));
                    if (next.StartsWith("0.", StringComparison.Ordinal))
                        clientDone = true;
                }

                back = true;
            }
        }
    }
}
