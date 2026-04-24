// Contrato del repositorio de zonas horarias: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de zonas horarias
public interface ITimeZoneRepository
{
    // Busca una zona horaria por su ID
    Task<AirlineTimeZone?> GetByIdAsync(TimeZoneId id, CancellationToken ct = default);

    // Retorna todas las zonas horarias registradas en el sistema
    Task<IReadOnlyList<AirlineTimeZone>> ListAsync(CancellationToken ct = default);

    // Agrega una nueva zona horaria al sistema
    Task AddAsync(AirlineTimeZone timeZone, CancellationToken ct = default);

    // Actualiza los datos de una zona horaria existente
    Task UpdateAsync(AirlineTimeZone timeZone, CancellationToken ct = default);

    // Elimina una zona horaria del sistema por su ID
    Task DeleteAsync(TimeZoneId id, CancellationToken ct = default);
}
