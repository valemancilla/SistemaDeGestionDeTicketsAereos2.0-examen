// Caso de uso: eliminar una tripulación por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Application.UseCases;

public sealed class DeleteCrewUseCase
{
    private readonly ICrewRepository _repo;
    public DeleteCrewUseCase(ICrewRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CrewId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(CrewId.Create(id), ct);
        return true;
    }
}
