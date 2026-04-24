// Caso de uso: obtener todos los aeropuertos registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;

public sealed class GetAllAirportsUseCase
{
    private readonly IAirportRepository _repo;

    public GetAllAirportsUseCase(IAirportRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin lógica adicional
    public async Task<IReadOnlyList<Airport>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
