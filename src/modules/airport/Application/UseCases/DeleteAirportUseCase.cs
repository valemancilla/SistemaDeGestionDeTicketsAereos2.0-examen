// Caso de uso: eliminar un aeropuerto por su ID, retorna false si no existe en lugar de lanzar excepción
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.UseCases;

public sealed class DeleteAirportUseCase
{
    private readonly IAirportRepository _repo;

    public DeleteAirportUseCase(IAirportRepository repo) => _repo = repo;

    // Retorna false en lugar de lanzar excepción — permite que la UI decida cómo manejar el caso
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(AirportId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(AirportId.Create(id), ct);
        return true;
    }
}
