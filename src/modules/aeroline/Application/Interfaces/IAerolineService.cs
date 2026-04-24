// Contrato del servicio de aerolíneas: define las operaciones de negocio disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.Interfaces;

// Interfaz del servicio de aerolíneas — desacopla la capa UI del servicio concreto
public interface IAerolineService
{
    // Crea una aerolínea nueva verificando que el código IATA no esté duplicado
    Task<Aeroline> CreateAsync(string name, string iataCode, int idCountry, bool active, CancellationToken cancellationToken = default);

    // Busca una aerolínea por su ID, retorna null si no existe
    Task<Aeroline?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todas las aerolíneas registradas en el sistema
    Task<IReadOnlyCollection<Aeroline>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de una aerolínea existente, lanza excepción si no se encuentra
    Task<Aeroline> UpdateAsync(int id, string name, string iataCode, int idCountry, bool active, CancellationToken cancellationToken = default);

    // Elimina una aerolínea por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
