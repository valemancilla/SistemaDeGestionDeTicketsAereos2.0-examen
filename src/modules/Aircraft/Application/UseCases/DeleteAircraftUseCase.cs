// Caso de uso: eliminar un avión por su ID, retorna false si no existe en lugar de lanzar excepción
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.UseCases;

public sealed class DeleteAircraftUseCase
{
    private readonly IAircraftRepository _repo;

    public DeleteAircraftUseCase(IAircraftRepository repo) => _repo = repo;

    // Retorna false en lugar de lanzar excepción — permite que la UI decida cómo manejar el caso
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(AircraftId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(AircraftId.Create(id), ct);
        return true;
    }
}
