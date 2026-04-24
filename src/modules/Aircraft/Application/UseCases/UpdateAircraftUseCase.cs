// Caso de uso: actualizar un avión existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.valueObject;
using AircraftClass = global::SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.aggregate.Aircraft;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.UseCases;

public sealed class UpdateAircraftUseCase
{
    private readonly IAircraftRepository _repo;

    public UpdateAircraftUseCase(IAircraftRepository repo) => _repo = repo;

    // Verifica que el avión exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<AircraftClass> ExecuteAsync(int id, int capacity, int idAirline, int idModel, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(AircraftId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Aircraft with id '{id}' was not found.");
        var updated = AircraftClass.Create(id, capacity, idAirline, idModel);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
