// Contrato del servicio de roles de empleado: define las operaciones disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Application.Interfaces;

// Interfaz del servicio de roles — desacopla la capa UI del servicio concreto
public interface IEmployeeRoleService
{
    // Crea un nuevo rol de empleado con el nombre dado
    Task<EmployeeRole> CreateAsync(string name, CancellationToken cancellationToken = default);

    // Busca un rol por su ID, retorna null si no existe
    Task<EmployeeRole?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los roles de empleado registrados
    Task<IReadOnlyCollection<EmployeeRole>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza el nombre de un rol existente, lanza excepción si no se encuentra
    Task<EmployeeRole> UpdateAsync(int id, string name, CancellationToken cancellationToken = default);

    // Elimina un rol por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
