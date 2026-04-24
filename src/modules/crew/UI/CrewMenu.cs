using SistemaDeGestionDeTicketsAereos.src.modules.employee.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.person.UI;
using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using Spectre.Console;

using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.UI;

public class CrewMenu : IModuleUI
{
    public string Key => "4";
    public string Title => "Gestión de Tripulación";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule($"[green]{Key}. {Title.ToUpper()}[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(6)
                    .AddChoices(
                        "1. Personas",
                        "2. Empleados",
                        "3. Tripulaciones (grupos y miembros)",
                        "0. Volver"));

            try
            {
                switch (option)
                {
                    case "1. Personas":             await new PersonMenu().RunAsync(cancellationToken);    break;
                    case "2. Empleados":             await new EmployeeMenu().RunAsync(cancellationToken);  break;
                    case "3. Tripulaciones (grupos y miembros)": await new CrewGroupMenu().RunAsync(cancellationToken); break;
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
