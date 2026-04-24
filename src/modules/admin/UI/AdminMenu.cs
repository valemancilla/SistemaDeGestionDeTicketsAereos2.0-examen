using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.city.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.country.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.role.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.UI;
using SistemaDeGestionDeTicketsAereos.src.modules.user.UI;
using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using Spectre.Console;

using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
namespace SistemaDeGestionDeTicketsAereos.src.modules.admin.UI;

public class AdminMenu : IModuleUI
{
    public string Key => "10";
    public string Title => "Administración (Catálogos)";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule($"[green]{Key}. {Title.ToUpper()}[/]").Centered());

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(16)
                    .AddChoiceGroup("--- GEOGRAFÍA ---", new[] {
                        "1. Países",
                        "2. Ciudades",
                    })
                    .AddChoiceGroup("--- CATÁLOGOS BASE ---", new[] {
                        "3. Géneros",
                        "4. Tipos de documento",
                        "5. Clases de asiento",
                        "6. Tipos de equipaje",
                        "7. Métodos de pago",
                        "8. Estados del sistema",
                        "9. Zonas horarias",
                        "10. Canales de check-in",
                    })
                    .AddChoiceGroup("--- PERSONAL Y SEGURIDAD ---", new[] {
                        "11. Roles de empleado",
                        "12. Roles de usuario",
                        "13. Usuarios del sistema",
                    })
                    .AddChoices(new[] { "0. Volver" })
            );

            if (option == "0. Volver") { back = true; continue; }

            try
            {
                await HandleOptionAsync(option, cancellationToken);
            }
            catch (Exception ex)
            {
                EntityPersistenceUiFeedback.Write(ex);
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
                Console.ReadKey();
            }
        }
    }

    private static async Task HandleOptionAsync(string option, CancellationToken ct)
    {
        switch (option)
        {
            case "1. Países":              await new CountryMenu().RunAsync(ct);        break;
            case "2. Ciudades":            await new CityMenu().RunAsync(ct);           break;
            case "3. Géneros":             await new GenderMenu().RunAsync(ct);         break;
            case "4. Tipos de documento":  await new DocumentTypeMenu().RunAsync(ct);   break;
            case "5. Clases de asiento":   await new SeatClassMenu().RunAsync(ct);      break;
            case "6. Tipos de equipaje":   await new BaggageTypeMenu().RunAsync(ct);    break;
            case "7. Métodos de pago":     await new PaymentMethodMenu().RunAsync(ct);  break;
            case "8. Estados del sistema": await new SystemStatusMenu().RunAsync(ct);   break;
            case "9. Zonas horarias":      await new TimeZoneMenu().RunAsync(ct);       break;
            case "10. Canales de check-in":await new CheckInChannelMenu().RunAsync(ct); break;
            case "11. Roles de empleado":  await new EmployeeRoleMenu().RunAsync(ct);   break;
            case "12. Roles de usuario":   await new RoleMenu().RunAsync(ct);           break;
            case "13. Usuarios del sistema":await new UserMenu().RunAsync(ct);          break;
        }
    }
}
