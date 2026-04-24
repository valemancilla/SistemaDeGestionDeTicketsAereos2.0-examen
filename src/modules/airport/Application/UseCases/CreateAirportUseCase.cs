// Caso de uso: registrar un nuevo aeropuerto verificando que el código IATA no esté duplicado
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;

public sealed class CreateAirportUseCase
{
    private readonly IAirportRepository _repo;

    public CreateAirportUseCase(IAirportRepository repo) => _repo = repo;

    // El código IATA identifica al aeropuerto mundialmente — debe ser único en el sistema
    public async Task<Airport> ExecuteAsync(string name, string iataCode, int idCity, bool active, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIataCodeAsync(iataCode, ct);
        if (existing is not null) throw new InvalidOperationException($"Airport with IATA code '{iataCode}' already exists.");
        var entity = Airport.CreateNew(name, iataCode, idCity, active);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
