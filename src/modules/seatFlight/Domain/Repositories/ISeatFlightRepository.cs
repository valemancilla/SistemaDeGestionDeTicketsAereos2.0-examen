// Contrato del repositorio de asignaciones asiento-vuelo: define las operaciones de persistencia
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de asignaciones asiento-vuelo
public interface ISeatFlightRepository
{
    // Busca una asignación por su ID
    Task<SeatFlight?> GetByIdAsync(SeatFlightId id, CancellationToken ct = default);

    // Busca una asignación por asiento y vuelo — combinación única que debe ser irrepetible
    Task<SeatFlight?> GetBySeatAndFlightAsync(int idSeat, int idFlight, CancellationToken ct = default);

    // Retorna todas las asignaciones del sistema
    Task<IReadOnlyList<SeatFlight>> ListAsync(CancellationToken ct = default);

    // Agrega una nueva asignación asiento-vuelo al sistema
    Task AddAsync(SeatFlight seatFlight, CancellationToken ct = default);

    // Actualiza una asignación existente (ej: marcar asiento como no disponible)
    Task UpdateAsync(SeatFlight seatFlight, CancellationToken ct = default);

    // Elimina una asignación del sistema por su ID
    Task DeleteAsync(SeatFlightId id, CancellationToken ct = default);
}
