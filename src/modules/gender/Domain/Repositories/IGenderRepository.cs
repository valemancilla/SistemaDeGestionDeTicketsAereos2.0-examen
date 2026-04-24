// Contrato del repositorio de géneros: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de géneros
public interface IGenderRepository
{
    // Busca un género por su ID
    Task<Gender?> GetByIdAsync(GenderId id, CancellationToken ct = default);

    // Retorna todos los géneros registrados en el sistema
    Task<IReadOnlyList<Gender>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo género al sistema
    Task AddAsync(Gender gender, CancellationToken ct = default);

    // Actualiza los datos de un género existente
    Task UpdateAsync(Gender gender, CancellationToken ct = default);

    // Elimina un género del sistema por su ID
    Task DeleteAsync(GenderId id, CancellationToken ct = default);
}
