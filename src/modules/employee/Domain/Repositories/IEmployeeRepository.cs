// Contrato del repositorio de empleados: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de empleados
public interface IEmployeeRepository
{
    // Busca un empleado por su ID
    Task<Employee?> GetByIdAsync(EmployeeId id, CancellationToken ct = default);

    // Busca el empleado asociado a una persona — útil para validar si ya existe
    Task<Employee?> GetByPersonIdAsync(int idPerson, CancellationToken ct = default);

    // Retorna todos los empleados del sistema
    Task<IReadOnlyList<Employee>> ListAsync(CancellationToken ct = default);

    // Retorna todos los empleados que pertenecen a una aerolínea específica
    Task<IReadOnlyList<Employee>> ListByAirlineAsync(int idAirline, CancellationToken ct = default);

    // Agrega un nuevo empleado al sistema
    Task AddAsync(Employee employee, CancellationToken ct = default);

    // Actualiza los datos de un empleado existente
    Task UpdateAsync(Employee employee, CancellationToken ct = default);

    // Elimina un empleado del sistema por su ID
    Task DeleteAsync(EmployeeId id, CancellationToken ct = default);
}
