// Caso de uso: obtener todas las relaciones aeropuerto-zona horaria del sistema
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Application.UseCases;

public sealed class GetAllAirportTimeZonesUseCase
{
    private readonly IAirportTimeZoneRepository _repo;

    public GetAllAirportTimeZonesUseCase(IAirportTimeZoneRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin lógica adicional
    public async Task<IReadOnlyList<AirportTimeZone>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
