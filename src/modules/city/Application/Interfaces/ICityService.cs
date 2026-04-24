// Contrato del servicio de ciudades: define las operaciones disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Application.Interfaces;

// Interfaz del servicio de ciudades — desacopla la capa UI del servicio concreto
public interface ICityService
{
    // Registra una nueva ciudad asociada a un país
    Task<City> CreateAsync(string name, int idCountry, CancellationToken cancellationToken = default);

    // Busca una ciudad por su ID, retorna null si no existe
    Task<City?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todas las ciudades registradas en el sistema
    Task<IReadOnlyCollection<City>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de una ciudad existente, lanza excepción si no se encuentra
    Task<City> UpdateAsync(int id, string name, int idCountry, CancellationToken cancellationToken = default);

    // Elimina una ciudad por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
