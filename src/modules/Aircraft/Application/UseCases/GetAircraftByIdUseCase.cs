// Caso de uso: buscar un avión por su ID, lanza excepción si no se encuentra
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.valueObject;
using AircraftClass = global::SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.aggregate.Aircraft;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.UseCases;

public sealed class GetAircraftByIdUseCase
{
    private readonly IAircraftRepository _repo;

    public GetAircraftByIdUseCase(IAircraftRepository repo) => _repo = repo;

    // Lanza KeyNotFoundException si el avión no existe, para que la UI pueda mostrar el mensaje apropiado
    public async Task<AircraftClass> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(AircraftId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Aircraft with id '{id}' was not found.");
        return entity;
    }
}
