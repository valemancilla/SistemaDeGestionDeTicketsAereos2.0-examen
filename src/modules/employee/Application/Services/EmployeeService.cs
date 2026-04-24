// Implementación del servicio de empleados: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Application.Services;

// Servicio de aplicación de empleados — orquesta sin lógica de dominio propia
public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public EmployeeService(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    // Una persona no puede ser empleado dos veces — se verifica por idPerson antes de persistir
    public async Task<Employee> CreateAsync(int idPerson, int idAirline, int idRole, CancellationToken cancellationToken = default)
    {
        var existing = await _employeeRepository.GetByPersonIdAsync(idPerson, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Employee for person '{idPerson}' already exists.");

        var entity = Employee.CreateNew(idPerson, idAirline, idRole);
        await _employeeRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un empleado por ID delegando directamente al repositorio
    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _employeeRepository.GetByIdAsync(EmployeeId.Create(id), cancellationToken);
    }

    // Retorna todos los empleados sin filtro
    public async Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _employeeRepository.ListAsync(cancellationToken);
    }

    // Actualiza un empleado verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<Employee> UpdateAsync(int id, int idPerson, int idAirline, int idRole, CancellationToken cancellationToken = default)
    {
        var employeeId = EmployeeId.Create(id);
        var existing = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Employee with id '{id}' was not found.");

        var updated = Employee.Create(id, idPerson, idAirline, idRole);
        await _employeeRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Retorna false si no existe — evita excepción cuando el recurso ya fue eliminado
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employeeId = EmployeeId.Create(id);
        var existing = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (existing is null)
            return false;

        await _employeeRepository.DeleteAsync(employeeId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
