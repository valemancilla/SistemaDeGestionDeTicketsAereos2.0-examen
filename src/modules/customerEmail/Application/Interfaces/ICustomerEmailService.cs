// Contrato del servicio de emails de cliente: define las operaciones para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Application.Interfaces;

// Interfaz del servicio de emails — desacopla la capa UI del servicio concreto
public interface ICustomerEmailService
{
    // Registra un nuevo email asociado a una persona
    Task<CustomerEmail> CreateAsync(string email, int idPerson, CancellationToken cancellationToken = default);

    // Busca un email por su ID, retorna null si no existe
    Task<CustomerEmail?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los emails de clientes registrados en el sistema
    Task<IReadOnlyCollection<CustomerEmail>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un email existente, lanza excepción si no se encuentra
    Task<CustomerEmail> UpdateAsync(int id, string email, int idPerson, CancellationToken cancellationToken = default);

    // Elimina un email por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
