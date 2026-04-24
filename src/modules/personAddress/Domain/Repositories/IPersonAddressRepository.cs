// Contrato del repositorio de direcciones de personas: define las operaciones de persistencia
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de direcciones de personas
public interface IPersonAddressRepository
{
    // Busca una dirección por su ID
    Task<PersonAddress?> GetByIdAsync(PersonAddressId id, CancellationToken ct = default);

    // Retorna todas las direcciones registradas en el sistema
    Task<IReadOnlyList<PersonAddress>> ListAsync(CancellationToken ct = default);

    // Retorna todas las direcciones de una persona específica (historial de direcciones)
    Task<IReadOnlyList<PersonAddress>> ListByPersonAsync(int idPerson, CancellationToken ct = default);

    // Agrega una nueva dirección al sistema
    Task AddAsync(PersonAddress address, CancellationToken ct = default);

    // Actualiza los datos de una dirección existente
    Task UpdateAsync(PersonAddress address, CancellationToken ct = default);

    // Elimina una dirección del sistema por su ID
    Task DeleteAsync(PersonAddressId id, CancellationToken ct = default);
}
