// Caso de uso: eliminar una aerolínea por su ID, retorna false si no existe en lugar de lanzar excepción
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.UseCases;

public sealed class DeleteAerolineUseCase
{
    private readonly IAirlineRepository _repo;

    public DeleteAerolineUseCase(IAirlineRepository repo) => _repo = repo;

    // Retorna false en lugar de lanzar excepción — permite que la UI decida cómo manejar el caso
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(AirlineId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(AirlineId.Create(id), ct);
        return true;
    }
}
