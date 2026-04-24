// Contrato del repositorio de roles de usuario: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de roles de usuario
public interface IRoleRepository
{
    // Busca un rol por su ID
    Task<Role?> GetByIdAsync(RoleId id, CancellationToken ct = default);

    // Retorna todos los roles del sistema
    Task<IReadOnlyList<Role>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo rol al sistema
    Task AddAsync(Role role, CancellationToken ct = default);

    // Actualiza los datos de un rol existente
    Task UpdateAsync(Role role, CancellationToken ct = default);

    // Elimina un rol del sistema por su ID
    Task DeleteAsync(RoleId id, CancellationToken ct = default);
}
