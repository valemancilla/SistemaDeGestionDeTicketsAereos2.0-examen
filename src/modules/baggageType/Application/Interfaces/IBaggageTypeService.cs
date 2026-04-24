// Contrato del servicio de tipos de equipaje: define las operaciones disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.Interfaces;

// Interfaz del servicio de tipos de equipaje — desacopla la capa UI del servicio concreto
public interface IBaggageTypeService
{
    // Crea un nuevo tipo de equipaje (ej: de mano, bodega, deportivo)
    Task<BaggageType> CreateAsync(string name, CancellationToken cancellationToken = default);

    // Busca un tipo por su ID, retorna null si no existe
    Task<BaggageType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los tipos de equipaje disponibles en el sistema
    Task<IReadOnlyCollection<BaggageType>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza el nombre de un tipo de equipaje existente, lanza excepción si no se encuentra
    Task<BaggageType> UpdateAsync(int id, string name, CancellationToken cancellationToken = default);

    // Elimina un tipo de equipaje por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
