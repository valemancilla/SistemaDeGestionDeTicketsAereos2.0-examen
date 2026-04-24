// Caso de uso: buscar la relación aeropuerto-zona horaria por los dos IDs
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Application.UseCases;

public sealed class GetAirportTimeZoneByIdUseCase
{
    private readonly IAirportTimeZoneRepository _repo;

    public GetAirportTimeZoneByIdUseCase(IAirportTimeZoneRepository repo) => _repo = repo;

    // Lanza KeyNotFoundException si la relación no existe, para que la UI pueda mostrar el mensaje apropiado
    public async Task<AirportTimeZone> ExecuteAsync(int idAirport, int idTimeZone, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(idAirport, idTimeZone, ct);
        if (entity is null) throw new KeyNotFoundException($"AirportTimeZone for airport '{idAirport}' and timeZone '{idTimeZone}' was not found.");
        return entity;
    }
}
