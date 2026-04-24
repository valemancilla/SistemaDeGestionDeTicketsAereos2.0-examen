// Caso de uso: asociar una zona horaria a un aeropuerto, verificando que la combinación no exista
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Application.UseCases;

public sealed class CreateAirportTimeZoneUseCase
{
    private readonly IAirportTimeZoneRepository _repo;

    public CreateAirportTimeZoneUseCase(IAirportTimeZoneRepository repo) => _repo = repo;

    // Un aeropuerto puede tener varias zonas horarias, pero la combinación específica debe ser única
    public async Task<AirportTimeZone> ExecuteAsync(int idAirport, int idTimeZone, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(idAirport, idTimeZone, ct);
        if (existing is not null) throw new InvalidOperationException($"AirportTimeZone for airport '{idAirport}' and timeZone '{idTimeZone}' already exists.");
        var entity = AirportTimeZone.Create(idAirport, idTimeZone);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
