// Contrato del repositorio de vuelos: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de vuelos
public interface IFlightRepository
{
    // Busca un vuelo por su ID
    Task<Flight?> GetByIdAsync(FlightId id, CancellationToken ct = default);

    // Busca un vuelo por número y fecha — combinación única que identifica un vuelo operado
    Task<Flight?> GetByFlightNumberAsync(string flightNumber, DateOnly date, CancellationToken ct = default);

    // Retorna todos los vuelos registrados en el sistema
    Task<IReadOnlyList<Flight>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo vuelo al sistema
    Task AddAsync(Flight flight, CancellationToken ct = default);

    // Actualiza los datos de un vuelo existente (ej: asientos disponibles tras una reserva)
    Task UpdateAsync(Flight flight, CancellationToken ct = default);

    // Elimina un vuelo del sistema por su ID
    Task DeleteAsync(FlightId id, CancellationToken ct = default);
}
