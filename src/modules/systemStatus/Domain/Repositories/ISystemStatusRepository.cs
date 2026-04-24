// Contrato del repositorio de estados del sistema: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de estados del sistema
public interface ISystemStatusRepository
{
    // Busca un estado por su ID
    Task<SystemStatus?> GetByIdAsync(SystemStatusId id, CancellationToken ct = default);

    // Retorna todos los estados del sistema
    Task<IReadOnlyList<SystemStatus>> ListAsync(CancellationToken ct = default);

    // Retorna los estados filtrados por tipo de entidad — útil para mostrar opciones según el contexto
    Task<IReadOnlyList<SystemStatus>> ListByEntityTypeAsync(string entityType, CancellationToken ct = default);

    // Agrega un nuevo estado al sistema
    Task AddAsync(SystemStatus systemStatus, CancellationToken ct = default);

    // Actualiza los datos de un estado existente
    Task UpdateAsync(SystemStatus systemStatus, CancellationToken ct = default);

    // Elimina un estado del sistema por su ID
    Task DeleteAsync(SystemStatusId id, CancellationToken ct = default);
}
