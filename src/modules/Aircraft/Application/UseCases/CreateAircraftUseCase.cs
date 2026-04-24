// Caso de uso: registrar un nuevo avión en el sistema con su capacidad, aerolínea y modelo
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.Repositories;
using AircraftClass = global::SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.aggregate.Aircraft;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.UseCases;

public sealed class CreateAircraftUseCase
{
    private readonly IAircraftRepository _repo;

    public CreateAircraftUseCase(IAircraftRepository repo) => _repo = repo;

    // Las validaciones de negocio (capacidad > 0, IDs válidos) las hace el agregado Aircraft.CreateNew
    public async Task<AircraftClass> ExecuteAsync(int capacity, int idAirline, int idModel, CancellationToken ct = default)
    {
        var entity = AircraftClass.CreateNew(capacity, idAirline, idModel);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
