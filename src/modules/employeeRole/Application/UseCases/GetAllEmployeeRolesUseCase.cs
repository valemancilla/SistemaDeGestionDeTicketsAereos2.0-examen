// Caso de uso: obtener todos los roles de empleado registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Application.UseCases;

public sealed class GetAllEmployeeRolesUseCase
{
    private readonly IEmployeeRoleRepository _repo;
    public GetAllEmployeeRolesUseCase(IEmployeeRoleRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<EmployeeRole>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
