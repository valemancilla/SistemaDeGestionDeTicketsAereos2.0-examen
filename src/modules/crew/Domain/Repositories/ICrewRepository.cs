// Contrato del repositorio de tripulaciones: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de tripulaciones
public interface ICrewRepository
{
    // Busca un grupo de tripulación por su ID
    Task<Crew?> GetByIdAsync(CrewId id, CancellationToken ct = default);

    // Retorna todos los grupos de tripulación registrados
    Task<IReadOnlyList<Crew>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo grupo de tripulación al sistema
    Task AddAsync(Crew crew, CancellationToken ct = default);

    // Actualiza los datos de un grupo de tripulación existente
    Task UpdateAsync(Crew crew, CancellationToken ct = default);

    // Elimina un grupo de tripulación por su ID
    Task DeleteAsync(CrewId id, CancellationToken ct = default);
}
