using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.UI;

public class TicketsMenu : IModuleUI
{
    public string Key => "7";
    public string Title => "Gestión de Tiquetes y Check-In";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await new TicketMenu().RunAsync(cancellationToken);
    }
}
