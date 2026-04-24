using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.UI;
using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using Spectre.Console;

using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.UI;

public class AirlinesMenu : IModuleUI
{
    public string Key => "1";
    public string Title => "Gestión de Aerolíneas y Aeronaves";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule($"[green]{Key}. {Title.ToUpper()}[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(12)
                    .AddChoiceGroup("--- CATÁLOGOS DE FLOTA ---", new[]
                    {
                        "1. Fabricantes",
                        "2. Modelos de aeronave",
                        "3. Clases de asiento (atajo)"
                    })
                    .AddChoiceGroup("--- OPERACIÓN ---", new[]
                    {
                        "4. Aerolíneas",
                        "5. Aeronaves y asientos"
                    })
                    .AddChoices("0. Volver"));

            try
            {
                switch (option)
                {
                    case "1. Fabricantes":            await new ManufacturerMenu().RunAsync(cancellationToken); break;
                    case "2. Modelos de aeronave":    await new AircraftModelMenu().RunAsync(cancellationToken); break;
                    case "3. Clases de asiento (atajo)": await new SeatClassMenu().RunAsync(cancellationToken); break;
                    case "4. Aerolíneas":             await new AerolineMenu().RunAsync(cancellationToken); break;
                    case "5. Aeronaves y asientos":   await new AircraftMenu().RunAsync(cancellationToken); break;
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
