// Caso de uso: eliminar un tipo de equipaje por su ID, retorna false si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.UseCases;

public sealed class DeleteBaggageTypeUseCase
{
    private readonly IBaggageTypeRepository _repo;

    public DeleteBaggageTypeUseCase(IBaggageTypeRepository repo) => _repo = repo;

    // Retorna false en lugar de lanzar excepción — permite que la UI decida cómo manejar el caso
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BaggageTypeId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(BaggageTypeId.Create(id), ct);
        return true;
    }
}
