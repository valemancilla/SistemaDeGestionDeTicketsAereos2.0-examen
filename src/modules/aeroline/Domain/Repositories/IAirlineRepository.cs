// Contrato del repositorio de aerolíneas: define qué operaciones de persistencia están disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de aerolíneas
public interface IAirlineRepository
{
    // Busca una aerolínea por su ID
    Task<Aeroline?> GetByIdAsync(AirlineId id, CancellationToken ct = default);

    // Busca una aerolínea por su código IATA (útil para verificar unicidad)
    Task<Aeroline?> GetByIataCodeAsync(string iataCode, CancellationToken ct = default);

    // Busca una aerolínea por su nombre (útil para verificar unicidad)
    Task<Aeroline?> GetByNameAsync(string name, CancellationToken ct = default);

    // Retorna todas las aerolíneas registradas en el sistema
    Task<IReadOnlyList<Aeroline>> ListAsync(CancellationToken ct = default);

    // Retorna solo las aerolíneas que están activas actualmente
    Task<IReadOnlyList<Aeroline>> ListActiveAsync(CancellationToken ct = default);

    // Agrega una nueva aerolínea al sistema
    Task AddAsync(Aeroline aeroline, CancellationToken ct = default);

    // Actualiza los datos de una aerolínea existente
    Task UpdateAsync(Aeroline aeroline, CancellationToken ct = default);

    // Elimina una aerolínea del sistema por su ID
    Task DeleteAsync(AirlineId id, CancellationToken ct = default);
}
