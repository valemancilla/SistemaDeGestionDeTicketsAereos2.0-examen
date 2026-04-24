// Contrato del repositorio de zonas horarias de aeropuertos
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.Repositories;

// Interfaz que define las operaciones del repositorio de AirportTimeZone
// Como la clave es compuesta (IdAirport + IdTimeZone), los métodos reciben los dos IDs
public interface IAirportTimeZoneRepository
{
    // Busca una relación aeropuerto-zona horaria específica por los dos IDs
    Task<AirportTimeZone?> GetByIdAsync(int idAirport, int idTimeZone, CancellationToken ct = default);

    // Retorna todas las relaciones aeropuerto-zona horaria del sistema
    Task<IReadOnlyList<AirportTimeZone>> ListAsync(CancellationToken ct = default);

    // Retorna todas las zonas horarias que tiene un aeropuerto específico
    Task<IReadOnlyList<AirportTimeZone>> ListByAirportAsync(int idAirport, CancellationToken ct = default);

    // Agrega una nueva relación aeropuerto-zona horaria
    Task AddAsync(AirportTimeZone airportTimeZone, CancellationToken ct = default);

    // Actualiza una relación existente
    Task UpdateAsync(AirportTimeZone airportTimeZone, CancellationToken ct = default);

    // Elimina una relación aeropuerto-zona horaria por los dos IDs
    Task RemoveAsync(int idAirport, int idTimeZone, CancellationToken ct = default);
}
