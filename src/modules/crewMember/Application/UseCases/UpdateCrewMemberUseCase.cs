// Caso de uso: actualizar un miembro de tripulación existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Application.UseCases;

public sealed class UpdateCrewMemberUseCase
{
    private readonly ICrewMemberRepository _repo;
    public UpdateCrewMemberUseCase(ICrewMemberRepository repo) => _repo = repo;

    // Verifica que el miembro exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<CrewMember> ExecuteAsync(int id, int idCrew, int idEmployee, int idRole, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CrewMemberId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"CrewMember with id '{id}' was not found.");
        var updated = CrewMember.Create(id, idCrew, idEmployee, idRole);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
