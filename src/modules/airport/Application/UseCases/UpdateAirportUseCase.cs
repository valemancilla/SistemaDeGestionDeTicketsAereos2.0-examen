// Caso de uso: actualizar un aeropuerto existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;

public sealed class UpdateAirportUseCase
{
    private readonly IAirportRepository _repo;

    public UpdateAirportUseCase(IAirportRepository repo) => _repo = repo;

    // Verifica que el aeropuerto exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<Airport> ExecuteAsync(int id, string name, string iataCode, int idCity, bool active, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(AirportId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Airport with id '{id}' was not found.");
        var updated = Airport.Create(id, name, iataCode, idCity, active);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
