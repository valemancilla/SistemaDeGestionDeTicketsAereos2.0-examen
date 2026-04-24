using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.Interfaces;

public interface IFlightService
{
    Task<Flight> CreateAsync(string number, DateOnly date, TimeOnly departureTime, TimeOnly arrivalTime, int totalCapacity, int availableSeats, int idRoute, int idAircraft, int idStatus, int idCrew, int idFare, CancellationToken cancellationToken = default);

    Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Flight>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Flight> UpdateAsync(int id, string number, DateOnly date, TimeOnly departureTime, TimeOnly arrivalTime, int totalCapacity, int availableSeats, int idRoute, int idAircraft, int idStatus, int idCrew, int idFare, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
