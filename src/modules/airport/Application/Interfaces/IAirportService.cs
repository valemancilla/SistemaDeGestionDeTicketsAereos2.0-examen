// Contrato del servicio de aeropuertos: define las operaciones de negocio disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.Interfaces;

// Interfaz del servicio de aeropuertos — desacopla la capa UI del servicio concreto
public interface IAirportService
{
    // Crea un aeropuerto nuevo verificando que el código IATA no esté duplicado
    Task<Airport> CreateAsync(string name, string iataCode, int idCity, bool active, CancellationToken cancellationToken = default);

    // Busca un aeropuerto por su ID, retorna null si no existe
    Task<Airport?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los aeropuertos registrados en el sistema
    Task<IReadOnlyCollection<Airport>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un aeropuerto existente, lanza excepción si no se encuentra
    Task<Airport> UpdateAsync(int id, string name, string iataCode, int idCity, bool active, CancellationToken cancellationToken = default);

    // Elimina un aeropuerto por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
