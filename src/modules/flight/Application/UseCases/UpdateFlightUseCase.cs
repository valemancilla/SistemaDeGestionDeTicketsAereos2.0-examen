using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.UseCases;

public sealed class UpdateFlightUseCase
{
    private readonly IFlightRepository _repo;
    public UpdateFlightUseCase(IFlightRepository repo) => _repo = repo;

    public async Task<Flight> ExecuteAsync(int id, string number, DateOnly date, TimeOnly departureTime, TimeOnly arrivalTime, int totalCapacity, int availableSeats, int idRoute, int idAircraft, int idStatus, int idCrew, int? idFare, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(FlightId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Flight with id '{id}' was not found.");
        var updated = Flight.Create(id, number, date, departureTime, arrivalTime, totalCapacity, availableSeats, idRoute, idAircraft, idStatus, idCrew, idFare);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
