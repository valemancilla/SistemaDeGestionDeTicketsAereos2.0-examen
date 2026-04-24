using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.UI;

public class CustomersMenu : IModuleUI
{
    public string Key => "5";
    public string Title => "Gestión de Clientes";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await new CustomerMenu().RunAsync(cancellationToken);
    }
}
