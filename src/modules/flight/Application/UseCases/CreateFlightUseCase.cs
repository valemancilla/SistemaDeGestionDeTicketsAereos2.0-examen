using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.UseCases;

public sealed class CreateFlightUseCase
{
    private readonly IFlightRepository _repo;
    public CreateFlightUseCase(IFlightRepository repo) => _repo = repo;

    public async Task<Flight> ExecuteAsync(string number, DateOnly date, TimeOnly departureTime, TimeOnly arrivalTime, int totalCapacity, int availableSeats, int idRoute, int idAircraft, int idStatus, int idCrew, int? idFare, CancellationToken ct = default)
    {
        var existing = await _repo.GetByFlightNumberAsync(number, date, ct);
        if (existing is not null) throw new InvalidOperationException($"Flight '{number}' on '{date}' already exists.");
        var entity = Flight.CreateNew(number, date, departureTime, arrivalTime, totalCapacity, availableSeats, idRoute, idAircraft, idStatus, idCrew, idFare);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
