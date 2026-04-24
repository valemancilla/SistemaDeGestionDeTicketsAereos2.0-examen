// Contrato del servicio de zonas horarias de aeropuerto: usa clave compuesta (idAirport + idTimeZone)
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Application.Interfaces;

// Interfaz del servicio de AirportTimeZone — los métodos reciben los dos IDs porque no hay ID propio
public interface IAirportTimeZoneService
{
    // Crea la relación aeropuerto-zona horaria verificando que no exista previamente
    Task<AirportTimeZone> CreateAsync(int idAirport, int idTimeZone, CancellationToken cancellationToken = default);

    // Busca la relación por los dos IDs, retorna null si no existe
    Task<AirportTimeZone?> GetByIdAsync(int idAirport, int idTimeZone, CancellationToken cancellationToken = default);

    // Retorna todas las relaciones aeropuerto-zona horaria del sistema
    Task<IReadOnlyCollection<AirportTimeZone>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza una relación existente identificada por los dos IDs
    Task<AirportTimeZone> UpdateAsync(int idAirport, int idTimeZone, CancellationToken cancellationToken = default);

    // Elimina la relación por los dos IDs, retorna false si no existe
    Task<bool> DeleteAsync(int idAirport, int idTimeZone, CancellationToken cancellationToken = default);
}
