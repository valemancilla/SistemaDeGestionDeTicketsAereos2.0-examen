// Caso de uso: obtener todas las aerolíneas registradas en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.UseCases;

public sealed class GetAllAerolinesUseCase
{
    private readonly IAirlineRepository _repo;

    public GetAllAerolinesUseCase(IAirlineRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin lógica adicional
    public async Task<IReadOnlyList<Aeroline>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
