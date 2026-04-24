using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.UI;

public class BookingsMenu : IModuleUI
{
    public string Key => "6";
    public string Title => "Gestión de Reservas";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await new BookingMenu().RunAsync(cancellationToken);
    }
}
