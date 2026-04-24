// Contrato del repositorio del historial de estados de vuelo: define las operaciones de persistencia
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.Repositories;

// Interfaz que define las operaciones del repositorio de historial de estados de vuelo
public interface IFlightStatusHistoryRepository
{
    // Busca un registro del historial por su ID
    Task<FlightStatusHistory?> GetByIdAsync(FlightStatusHistoryId id, CancellationToken ct = default);

    // Retorna todos los registros del historial en el sistema
    Task<IReadOnlyList<FlightStatusHistory>> ListAsync(CancellationToken ct = default);

    // Retorna todos los cambios de estado de un vuelo específico — útil para auditoría
    Task<IReadOnlyList<FlightStatusHistory>> ListByFlightAsync(int idFlight, CancellationToken ct = default);

    // Agrega un nuevo registro al historial
    Task AddAsync(FlightStatusHistory history, CancellationToken ct = default);

    // Actualiza un registro existente del historial
    Task UpdateAsync(FlightStatusHistory history, CancellationToken ct = default);

    // Elimina un registro del historial por su ID
    Task DeleteAsync(FlightStatusHistoryId id, CancellationToken ct = default);
}
