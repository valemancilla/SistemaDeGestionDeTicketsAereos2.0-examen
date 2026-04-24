// Contrato del servicio de tripulaciones: define las operaciones disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Application.Interfaces;

// Interfaz del servicio de tripulaciones — desacopla la capa UI del servicio concreto
public interface ICrewService
{
    // Registra una nueva tripulación con su nombre de grupo
    Task<Crew> CreateAsync(string groupName, CancellationToken cancellationToken = default);

    // Busca una tripulación por su ID, retorna null si no existe
    Task<Crew?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todas las tripulaciones registradas en el sistema
    Task<IReadOnlyCollection<Crew>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza el nombre de una tripulación existente, lanza excepción si no se encuentra
    Task<Crew> UpdateAsync(int id, string groupName, CancellationToken cancellationToken = default);

    // Elimina una tripulación por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
