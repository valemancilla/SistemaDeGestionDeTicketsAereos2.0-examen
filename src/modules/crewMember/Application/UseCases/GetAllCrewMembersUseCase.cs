// Caso de uso: obtener todos los miembros de tripulación registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Application.UseCases;

public sealed class GetAllCrewMembersUseCase
{
    private readonly ICrewMemberRepository _repo;
    public GetAllCrewMembersUseCase(ICrewMemberRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<CrewMember>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
