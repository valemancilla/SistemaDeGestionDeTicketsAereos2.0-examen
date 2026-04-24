// Caso de uso: eliminar un rol de empleado por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Application.UseCases;

public sealed class DeleteEmployeeRoleUseCase
{
    private readonly IEmployeeRoleRepository _repo;
    public DeleteEmployeeRoleUseCase(IEmployeeRoleRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(EmployeeRoleId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(EmployeeRoleId.Create(id), ct);
        return true;
    }
}
