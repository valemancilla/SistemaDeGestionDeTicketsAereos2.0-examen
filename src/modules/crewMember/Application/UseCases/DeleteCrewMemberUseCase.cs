// Caso de uso: eliminar un miembro de tripulación por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Application.UseCases;

public sealed class DeleteCrewMemberUseCase
{
    private readonly ICrewMemberRepository _repo;
    public DeleteCrewMemberUseCase(ICrewMemberRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CrewMemberId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(CrewMemberId.Create(id), ct);
        return true;
    }
}
