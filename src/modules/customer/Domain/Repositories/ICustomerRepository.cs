// Contrato del repositorio de clientes: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de clientes
public interface ICustomerRepository
{
    // Busca un cliente por su ID
    Task<Customer?> GetByIdAsync(CustomerId id, CancellationToken ct = default);

    // Busca el cliente asociado a una persona — útil para validar si ya existe
    Task<Customer?> GetByPersonIdAsync(int idPerson, CancellationToken ct = default);

    // Retorna todos los clientes registrados en el sistema
    Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo cliente al sistema
    Task AddAsync(Customer customer, CancellationToken ct = default);

    // Actualiza los datos de un cliente existente
    Task UpdateAsync(Customer customer, CancellationToken ct = default);

    // Elimina un cliente del sistema por su ID
    Task DeleteAsync(CustomerId id, CancellationToken ct = default);
}
