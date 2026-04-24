// Caso de uso: registrar un nuevo rol de empleado en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Application.UseCases;

public sealed class CreateEmployeeRoleUseCase
{
    private readonly IEmployeeRoleRepository _repo;
    public CreateEmployeeRoleUseCase(IEmployeeRoleRepository repo) => _repo = repo;

    // Crea el rol con nombre válido y lo persiste a través del repositorio
    public async Task<EmployeeRole> ExecuteAsync(string name, CancellationToken ct = default)
    {
        var entity = EmployeeRole.CreateNew(name);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
