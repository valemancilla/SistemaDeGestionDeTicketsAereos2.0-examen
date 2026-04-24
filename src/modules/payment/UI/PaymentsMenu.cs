using SistemaDeGestionDeTicketsAereos.src.shared.ui;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.UI;

public class PaymentsMenu : IModuleUI
{
    public string Key => "8";
    public string Title => "Gestión de Pagos";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await new PaymentMenu().RunAsync(cancellationToken);
    }
}
