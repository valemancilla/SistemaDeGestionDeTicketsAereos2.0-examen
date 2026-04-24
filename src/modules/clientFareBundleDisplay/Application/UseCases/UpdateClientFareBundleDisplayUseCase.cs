using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Domain;
using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Infrastructure.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Application.UseCases;

public sealed class UpdateClientFareBundleDisplayUseCase
{
    private readonly ClientFareBundleDisplayRepository _repo;
    public UpdateClientFareBundleDisplayUseCase(ClientFareBundleDisplayRepository repo) => _repo = repo;

    public async Task ExecuteAsync(ClientFareBundleDisplayData data, CancellationToken ct = default)
    {
        if (data.ClassicMultiplier <= 0 || data.FlexMultiplier <= 0)
            throw new InvalidOperationException("Los multiplicadores deben ser mayores a 0.");
        if (data.RefCarryOnCop < 0 || data.RefCheckedCop < 0)
            throw new InvalidOperationException("Los montos de referencia de equipaje no pueden ser negativos.");
        await _repo.UpdateAsync(data, ct);
    }
}
