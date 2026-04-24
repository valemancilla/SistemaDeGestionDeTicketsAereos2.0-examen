// Implementación del servicio de roles de empleado: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Application.Services;

// Servicio de aplicación de roles — orquesta sin lógica de dominio propia
public sealed class EmployeeRoleService : IEmployeeRoleService
{
    private readonly IEmployeeRoleRepository _employeeRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public EmployeeRoleService(IEmployeeRoleRepository employeeRoleRepository, IUnitOfWork unitOfWork)
    {
        _employeeRoleRepository = employeeRoleRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea el rol y persiste a través del UoW
    public async Task<EmployeeRole> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = EmployeeRole.CreateNew(name);
        await _employeeRoleRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un rol por ID delegando directamente al repositorio
    public Task<EmployeeRole?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _employeeRoleRepository.GetByIdAsync(EmployeeRoleId.Create(id), cancellationToken);
    }

    // Retorna todos los roles sin filtro
    public async Task<IReadOnlyCollection<EmployeeRole>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _employeeRoleRepository.ListAsync(cancellationToken);
    }

    // Actualiza un rol verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<EmployeeRole> UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        var employeeRoleId = EmployeeRoleId.Create(id);
        var existing = await _employeeRoleRepository.GetByIdAsync(employeeRoleId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"EmployeeRole with id '{id}' was not found.");

        var updated = EmployeeRole.Create(id, name);
        await _employeeRoleRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Retorna false si no existe — evita excepción cuando el recurso ya fue eliminado
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employeeRoleId = EmployeeRoleId.Create(id);
        var existing = await _employeeRoleRepository.GetByIdAsync(employeeRoleId, cancellationToken);
        if (existing is null)
            return false;

        await _employeeRoleRepository.DeleteAsync(employeeRoleId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
