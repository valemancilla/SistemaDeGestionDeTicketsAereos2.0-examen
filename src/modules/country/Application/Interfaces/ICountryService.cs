// Contrato del servicio de países: define las operaciones disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Application.Interfaces;

// Interfaz del servicio de países — desacopla la capa UI del servicio concreto
public interface ICountryService
{
    // Registra un nuevo país verificando que el código ISO no esté duplicado
    Task<Country> CreateAsync(string name, string isoCode, CancellationToken cancellationToken = default);

    // Busca un país por su ID, retorna null si no existe
    Task<Country?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los países registrados en el sistema
    Task<IReadOnlyCollection<Country>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un país existente, lanza excepción si no se encuentra
    Task<Country> UpdateAsync(int id, string name, string isoCode, CancellationToken cancellationToken = default);

    // Elimina un país por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
