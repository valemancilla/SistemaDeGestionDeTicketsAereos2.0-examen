// Caso de uso: registrar una nueva tripulación con su nombre de grupo
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Application.UseCases;

public sealed class CreateCrewUseCase
{
    private readonly ICrewRepository _repo;
    public CreateCrewUseCase(ICrewRepository repo) => _repo = repo;

    // La validación del nombre la hace el agregado
    public async Task<Crew> ExecuteAsync(string groupName, CancellationToken ct = default)
    {
        var entity = Crew.CreateNew(groupName);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
