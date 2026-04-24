// Caso de uso: eliminar la relación aeropuerto-zona horaria por los dos IDs
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Application.UseCases;

public sealed class DeleteAirportTimeZoneUseCase
{
    private readonly IAirportTimeZoneRepository _repo;

    public DeleteAirportTimeZoneUseCase(IAirportTimeZoneRepository repo) => _repo = repo;

    // Retorna false en lugar de lanzar excepción — permite que la UI decida cómo manejar el caso
    public async Task<bool> ExecuteAsync(int idAirport, int idTimeZone, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(idAirport, idTimeZone, ct);
        if (existing is null) return false;
        await _repo.RemoveAsync(idAirport, idTimeZone, ct);
        return true;
    }
}
