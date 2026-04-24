using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.Interfaces;

public interface ISeatService
{
    Task<Seat> CreateAsync(string number, int idAircraft, int idClase, CancellationToken cancellationToken = default);

    Task<Seat?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Seat>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Seat> UpdateAsync(int id, string number, int idAircraft, int idClase, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
