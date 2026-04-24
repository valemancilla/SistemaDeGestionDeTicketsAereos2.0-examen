// Caso de uso: buscar una tripulación por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Application.UseCases;

public sealed class GetCrewByIdUseCase
{
    private readonly ICrewRepository _repo;
    public GetCrewByIdUseCase(ICrewRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<Crew> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(CrewId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Crew with id '{id}' was not found.");
        return entity;
    }
}
