// Caso de uso: eliminar un equipaje por su ID, retorna false si no existe en lugar de lanzar excepción
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.UseCases;

public sealed class DeleteBaggageUseCase
{
    private readonly IBaggageRepository _repo;

    public DeleteBaggageUseCase(IBaggageRepository repo) => _repo = repo;

    // Retorna false en lugar de lanzar excepción — permite que la UI decida cómo manejar el caso
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BaggageId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(BaggageId.Create(id), ct);
        return true;
    }
}
