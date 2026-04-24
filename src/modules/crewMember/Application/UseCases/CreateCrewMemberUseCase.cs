// Caso de uso: vincular un empleado a una tripulación con un rol específico
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Application.UseCases;

public sealed class CreateCrewMemberUseCase
{
    private readonly ICrewMemberRepository _repo;
    public CreateCrewMemberUseCase(ICrewMemberRepository repo) => _repo = repo;

    // Las validaciones de FKs las hace el agregado
    public async Task<CrewMember> ExecuteAsync(int idCrew, int idEmployee, int idRole, CancellationToken ct = default)
    {
        var entity = CrewMember.CreateNew(idCrew, idEmployee, idRole);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
