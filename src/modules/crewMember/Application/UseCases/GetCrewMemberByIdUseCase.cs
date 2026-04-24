// Caso de uso: buscar un miembro de tripulación por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Application.UseCases;

public sealed class GetCrewMemberByIdUseCase
{
    private readonly ICrewMemberRepository _repo;
    public GetCrewMemberByIdUseCase(ICrewMemberRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<CrewMember> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(CrewMemberId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"CrewMember with id '{id}' was not found.");
        return entity;
    }
}
