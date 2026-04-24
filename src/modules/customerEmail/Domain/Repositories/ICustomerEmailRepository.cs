// Contrato del repositorio de correos de clientes: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de correos de clientes
public interface ICustomerEmailRepository
{
    // Busca un correo por su ID
    Task<CustomerEmail?> GetByIdAsync(CustomerEmailId id, CancellationToken ct = default);

    // Retorna todos los correos registrados en el sistema
    Task<IReadOnlyList<CustomerEmail>> ListAsync(CancellationToken ct = default);

    // Retorna todos los correos de una persona específica
    Task<IReadOnlyList<CustomerEmail>> ListByPersonAsync(int idPerson, CancellationToken ct = default);

    // Agrega un nuevo correo al sistema
    Task AddAsync(CustomerEmail email, CancellationToken ct = default);

    // Actualiza los datos de un correo existente
    Task UpdateAsync(CustomerEmail email, CancellationToken ct = default);

    // Elimina un correo del sistema por su ID
    Task DeleteAsync(CustomerEmailId id, CancellationToken ct = default);

    // True si el correo ya está registrado (otra fila; al actualizar, excluir IdEmail)
    Task<bool> IsEmailInUseAsync(string email, int? exceptIdEmail, CancellationToken ct = default);
}
