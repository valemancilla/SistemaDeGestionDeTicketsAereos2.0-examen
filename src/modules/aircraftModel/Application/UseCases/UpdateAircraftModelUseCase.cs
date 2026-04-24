// Caso de uso: actualizar un modelo de aeronave existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Application.UseCases;

public sealed class UpdateAircraftModelUseCase
{
    private readonly IAircraftModelRepository _repo;

    public UpdateAircraftModelUseCase(IAircraftModelRepository repo) => _repo = repo;

    // Verifica que el modelo exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<AircraftModel> ExecuteAsync(int id, string name, int idManufacturer, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(AircraftModelId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"AircraftModel with id '{id}' was not found.");
        var updated = AircraftModel.Create(id, name, idManufacturer);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
