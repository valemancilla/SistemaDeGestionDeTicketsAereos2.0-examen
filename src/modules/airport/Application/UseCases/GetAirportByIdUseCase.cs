// Caso de uso: buscar un aeropuerto por su ID, lanza excepción si no se encuentra
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;

public sealed class GetAirportByIdUseCase
{
    private readonly IAirportRepository _repo;

    public GetAirportByIdUseCase(IAirportRepository repo) => _repo = repo;

    // Lanza KeyNotFoundException si el aeropuerto no existe, para que la UI pueda mostrar el mensaje apropiado
    public async Task<Airport> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(AirportId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Airport with id '{id}' was not found.");
        return entity;
    }
}
