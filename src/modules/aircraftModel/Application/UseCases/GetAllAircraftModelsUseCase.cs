// Caso de uso: obtener todos los modelos de aeronave registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Application.UseCases;

public sealed class GetAllAircraftModelsUseCase
{
    private readonly IAircraftModelRepository _repo;

    public GetAllAircraftModelsUseCase(IAircraftModelRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin lógica adicional
    public async Task<IReadOnlyList<AircraftModel>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
