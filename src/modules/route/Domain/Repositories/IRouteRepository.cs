// Contrato del repositorio de rutas aéreas: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de rutas
public interface IRouteRepository
{
    // Busca una ruta por su ID
    Task<Route?> GetByIdAsync(RouteId id, CancellationToken ct = default);

    // Retorna todas las rutas del sistema (activas e inactivas)
    Task<IReadOnlyList<Route>> ListAsync(CancellationToken ct = default);

    // Retorna solo las rutas que están operativas actualmente — útil para mostrar opciones de vuelo
    Task<IReadOnlyList<Route>> ListActiveAsync(CancellationToken ct = default);

    // Agrega una nueva ruta al sistema
    Task AddAsync(Route route, CancellationToken ct = default);

    // Actualiza los datos de una ruta existente
    Task UpdateAsync(Route route, CancellationToken ct = default);

    // Elimina una ruta del sistema por su ID
    Task DeleteAsync(RouteId id, CancellationToken ct = default);
}
