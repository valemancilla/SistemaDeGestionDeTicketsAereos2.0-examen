// Contrato del repositorio de aviones: define qué operaciones de persistencia están disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.valueObject;
using AircraftClass = global::SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.aggregate.Aircraft;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de aviones
// El alias AircraftClass es necesario porque el namespace "Aircraft" colisiona con el tipo "Aircraft"
public interface IAircraftRepository
{
    // Busca un avión por su ID
    Task<AircraftClass?> GetByIdAsync(AircraftId id, CancellationToken ct = default);

    // Retorna todos los aviones del sistema
    Task<IReadOnlyList<AircraftClass>> ListAsync(CancellationToken ct = default);

    // Retorna todos los aviones que pertenecen a una aerolínea específica
    Task<IReadOnlyList<AircraftClass>> ListByAirlineAsync(int idAirline, CancellationToken ct = default);

    // Agrega un nuevo avión al sistema
    Task AddAsync(AircraftClass aircraft, CancellationToken ct = default);

    // Actualiza los datos de un avión existente
    Task UpdateAsync(AircraftClass aircraft, CancellationToken ct = default);

    // Elimina un avión del sistema por su ID
    Task DeleteAsync(AircraftId id, CancellationToken ct = default);
}
