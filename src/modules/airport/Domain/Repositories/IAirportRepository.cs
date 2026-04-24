// Contrato del repositorio de aeropuertos: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de aeropuertos
public interface IAirportRepository
{
    // Busca un aeropuerto por su ID
    Task<Airport?> GetByIdAsync(AirportId id, CancellationToken ct = default);

    // Busca un aeropuerto por su código IATA (útil para verificar unicidad)
    Task<Airport?> GetByIataCodeAsync(string iataCode, CancellationToken ct = default);

    // Retorna todos los aeropuertos registrados en el sistema
    Task<IReadOnlyList<Airport>> ListAsync(CancellationToken ct = default);

    // Retorna solo los aeropuertos que están activos actualmente
    Task<IReadOnlyList<Airport>> ListActiveAsync(CancellationToken ct = default);

    // Agrega un nuevo aeropuerto al sistema
    Task AddAsync(Airport airport, CancellationToken ct = default);

    // Actualiza los datos de un aeropuerto existente
    Task UpdateAsync(Airport airport, CancellationToken ct = default);

    // Elimina un aeropuerto del sistema por su ID
    Task DeleteAsync(AirportId id, CancellationToken ct = default);
}
