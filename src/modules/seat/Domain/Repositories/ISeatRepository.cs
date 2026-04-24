// Contrato del repositorio de asientos: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de asientos
public interface ISeatRepository
{
    // Busca un asiento por su ID
    Task<Seat?> GetByIdAsync(SeatId id, CancellationToken ct = default);

    // Retorna todos los asientos del sistema
    Task<IReadOnlyList<Seat>> ListAsync(CancellationToken ct = default);

    // Retorna todos los asientos de una aeronave específica
    Task<IReadOnlyList<Seat>> ListByAircraftAsync(int idAircraft, CancellationToken ct = default);

    // Agrega un nuevo asiento al sistema
    Task AddAsync(Seat seat, CancellationToken ct = default);

    // Actualiza los datos de un asiento existente
    Task UpdateAsync(Seat seat, CancellationToken ct = default);

    // Elimina un asiento del sistema por su ID
    Task DeleteAsync(SeatId id, CancellationToken ct = default);
}
