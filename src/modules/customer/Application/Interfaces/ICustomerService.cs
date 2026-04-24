// Contrato del servicio de clientes: define las operaciones disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Application.Interfaces;

// Interfaz del servicio de clientes — desacopla la capa UI del servicio concreto
public interface ICustomerService
{
    // Registra un nuevo cliente vinculado a una persona — una persona solo puede ser cliente una vez
    Task<Customer> CreateAsync(DateOnly registrationDate, int idPerson, bool active, CancellationToken cancellationToken = default);

    // Busca un cliente por su ID, retorna null si no existe
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los clientes registrados en el sistema
    Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un cliente existente, lanza excepción si no se encuentra
    Task<Customer> UpdateAsync(int id, DateOnly registrationDate, int idPerson, bool active, CancellationToken cancellationToken = default);

    // Elimina un cliente por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
