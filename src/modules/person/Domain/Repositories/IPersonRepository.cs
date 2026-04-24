// Contrato del repositorio de personas: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de personas
public interface IPersonRepository
{
    // Busca una persona por su ID
    Task<Person?> GetByIdAsync(PersonId id, CancellationToken ct = default);

    // Busca una persona por tipo y número de documento (criterio de búsqueda)
    Task<Person?> GetByDocumentAsync(int idDocumentType, string documentNumber, CancellationToken ct = default);

    // Busca por número de documento ya normalizado (ver PersonDocumentNumber) — el número es único en todo el sistema
    Task<Person?> GetByDocumentNumberAsync(string normalizedDocumentNumber, CancellationToken ct = default);

    // Retorna todas las personas registradas en el sistema
    Task<IReadOnlyList<Person>> ListAsync(CancellationToken ct = default);

    // Agrega una nueva persona al sistema
    Task AddAsync(Person person, CancellationToken ct = default);

    // Actualiza los datos de una persona existente
    Task UpdateAsync(Person person, CancellationToken ct = default);

    // Elimina una persona del sistema por su ID
    Task DeleteAsync(PersonId id, CancellationToken ct = default);
}
