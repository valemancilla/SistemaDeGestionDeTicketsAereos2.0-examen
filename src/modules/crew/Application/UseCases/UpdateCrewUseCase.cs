// Caso de uso: actualizar una tripulación existente verificando que exista antes de modificarla
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Application.UseCases;

public sealed class UpdateCrewUseCase
{
    private readonly ICrewRepository _repo;
    public UpdateCrewUseCase(ICrewRepository repo) => _repo = repo;

    // Verifica que la tripulación exista antes de actualizarla — recrea el agregado con los nuevos datos
    public async Task<Crew> ExecuteAsync(int id, string groupName, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CrewId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Crew with id '{id}' was not found.");
        var updated = Crew.Create(id, groupName);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
