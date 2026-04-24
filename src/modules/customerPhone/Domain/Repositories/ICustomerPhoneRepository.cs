// Contrato del repositorio de teléfonos de clientes: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de teléfonos de clientes
public interface ICustomerPhoneRepository
{
    // Busca un teléfono por su ID
    Task<CustomerPhone?> GetByIdAsync(CustomerPhoneId id, CancellationToken ct = default);

    // Retorna todos los teléfonos registrados en el sistema
    Task<IReadOnlyList<CustomerPhone>> ListAsync(CancellationToken ct = default);

    // Retorna todos los teléfonos de una persona específica
    Task<IReadOnlyList<CustomerPhone>> ListByPersonAsync(int idPerson, CancellationToken ct = default);

    // Agrega un nuevo teléfono al sistema
    Task AddAsync(CustomerPhone phone, CancellationToken ct = default);

    // Actualiza los datos de un teléfono existente
    Task UpdateAsync(CustomerPhone phone, CancellationToken ct = default);

    // Elimina un teléfono del sistema por su ID
    Task DeleteAsync(CustomerPhoneId id, CancellationToken ct = default);

    // True si el teléfono ya está registrado (otra fila; al actualizar, excluir IdPhone)
    Task<bool> IsPhoneInUseAsync(string phone, int? exceptIdPhone, CancellationToken ct = default);
}
