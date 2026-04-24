using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.Interfaces;

public interface ISeatFlightService
{
    Task<SeatFlight> CreateAsync(int idSeat, int idFlight, bool available, CancellationToken cancellationToken = default);

    Task<SeatFlight?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SeatFlight>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SeatFlight> UpdateAsync(int id, int idSeat, int idFlight, bool available, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
