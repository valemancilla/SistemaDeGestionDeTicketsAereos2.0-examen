// Caso de uso: eliminar un modelo de aeronave por su ID, retorna false si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Application.UseCases;

public sealed class DeleteAircraftModelUseCase
{
    private readonly IAircraftModelRepository _repo;

    public DeleteAircraftModelUseCase(IAircraftModelRepository repo) => _repo = repo;

    // Retorna false en lugar de lanzar excepción — permite que la UI decida cómo manejar el caso
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(AircraftModelId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(AircraftModelId.Create(id), ct);
        return true;
    }
}
