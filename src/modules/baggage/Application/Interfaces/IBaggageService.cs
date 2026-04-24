// Contrato del servicio de equipajes: define las operaciones de negocio disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.Interfaces;

// Interfaz del servicio de equipajes — desacopla la capa UI del servicio concreto
public interface IBaggageService
{
    // Crea un nuevo equipaje asociado a un tiquete y tipo específico
    Task<Baggage> CreateAsync(decimal weight, int idTicket, int idBaggageType, CancellationToken cancellationToken = default);

    // Busca un equipaje por su ID, retorna null si no existe
    Task<Baggage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los equipajes registrados en el sistema
    Task<IReadOnlyCollection<Baggage>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un equipaje existente, lanza excepción si no se encuentra
    Task<Baggage> UpdateAsync(int id, decimal weight, int idTicket, int idBaggageType, CancellationToken cancellationToken = default);

    // Elimina un equipaje por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
