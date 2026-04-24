// Contrato del servicio de empleados: define las operaciones disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Application.Interfaces;

// Interfaz del servicio de empleados — desacopla la capa UI del servicio concreto
public interface IEmployeeService
{
    // Registra un nuevo empleado vinculado a una persona — una persona solo puede ser empleado una vez
    Task<Employee> CreateAsync(int idPerson, int idAirline, int idRole, CancellationToken cancellationToken = default);

    // Busca un empleado por su ID, retorna null si no existe
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los empleados registrados en el sistema
    Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un empleado existente, lanza excepción si no se encuentra
    Task<Employee> UpdateAsync(int id, int idPerson, int idAirline, int idRole, CancellationToken cancellationToken = default);

    // Elimina un empleado por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
