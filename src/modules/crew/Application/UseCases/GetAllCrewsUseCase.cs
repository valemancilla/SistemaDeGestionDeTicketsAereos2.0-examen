// Caso de uso: obtener todas las tripulaciones registradas en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Application.UseCases;

public sealed class GetAllCrewsUseCase
{
    private readonly ICrewRepository _repo;
    public GetAllCrewsUseCase(ICrewRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<Crew>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
