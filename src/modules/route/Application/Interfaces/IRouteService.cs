using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Application.Interfaces;

public interface IRouteService
{
    Task<Route> CreateAsync(decimal distanceKm, TimeOnly estDuration, int originAirport, int destinationAirport, bool active, CancellationToken cancellationToken = default);

    Task<Route?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Route>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Route> UpdateAsync(int id, decimal distanceKm, TimeOnly estDuration, int originAirport, int destinationAirport, bool active, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
