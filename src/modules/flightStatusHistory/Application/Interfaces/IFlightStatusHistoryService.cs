using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Application.Interfaces;

public interface IFlightStatusHistoryService
{
    Task<FlightStatusHistory> CreateAsync(DateTime changeDate, string? observation, int idFlight, int idStatus, int idUser, CancellationToken cancellationToken = default);

    Task<FlightStatusHistory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FlightStatusHistory>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<FlightStatusHistory> UpdateAsync(int id, DateTime changeDate, string? observation, int idFlight, int idStatus, int idUser, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
