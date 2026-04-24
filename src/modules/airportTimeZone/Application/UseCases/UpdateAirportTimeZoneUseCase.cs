// Caso de uso: actualizar la relación aeropuerto-zona horaria verificando que exista
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Application.UseCases;

public sealed class UpdateAirportTimeZoneUseCase
{
    private readonly IAirportTimeZoneRepository _repo;

    public UpdateAirportTimeZoneUseCase(IAirportTimeZoneRepository repo) => _repo = repo;

    // Verifica que la relación exista antes de actualizarla — recrea el agregado con los nuevos datos
    public async Task<AirportTimeZone> ExecuteAsync(int idAirport, int idTimeZone, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(idAirport, idTimeZone, ct);
        if (existing is null) throw new KeyNotFoundException($"AirportTimeZone for airport '{idAirport}' and timeZone '{idTimeZone}' was not found.");
        var updated = AirportTimeZone.Create(idAirport, idTimeZone);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
