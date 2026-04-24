// Contrato del servicio de teléfonos de cliente: define las operaciones para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Application.Interfaces;

// Interfaz del servicio de teléfonos — desacopla la capa UI del servicio concreto
public interface ICustomerPhoneService
{
    // Registra un nuevo teléfono asociado a una persona
    Task<CustomerPhone> CreateAsync(string phone, int idPerson, CancellationToken cancellationToken = default);

    // Busca un teléfono por su ID, retorna null si no existe
    Task<CustomerPhone?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los teléfonos de clientes registrados en el sistema
    Task<IReadOnlyCollection<CustomerPhone>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un teléfono existente, lanza excepción si no se encuentra
    Task<CustomerPhone> UpdateAsync(int id, string phone, int idPerson, CancellationToken cancellationToken = default);

    // Elimina un teléfono por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
