// Caso de uso: obtener todos los aviones registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.Repositories;
using AircraftClass = global::SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.aggregate.Aircraft;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.UseCases;

public sealed class GetAllAircraftsUseCase
{
    private readonly IAircraftRepository _repo;

    public GetAllAircraftsUseCase(IAircraftRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin lógica adicional
    public async Task<IReadOnlyList<AircraftClass>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
