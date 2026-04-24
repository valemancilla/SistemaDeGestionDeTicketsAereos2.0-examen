// Contrato del repositorio de roles de empleado: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de roles de empleado
public interface IEmployeeRoleRepository
{
    // Busca un rol por su ID
    Task<EmployeeRole?> GetByIdAsync(EmployeeRoleId id, CancellationToken ct = default);

    // Retorna todos los roles de empleado registrados
    Task<IReadOnlyList<EmployeeRole>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo rol de empleado al sistema
    Task AddAsync(EmployeeRole employeeRole, CancellationToken ct = default);

    // Actualiza los datos de un rol existente
    Task UpdateAsync(EmployeeRole employeeRole, CancellationToken ct = default);

    // Elimina un rol de empleado del sistema por su ID
    Task DeleteAsync(EmployeeRoleId id, CancellationToken ct = default);
}
