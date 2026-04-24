using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Application.UseCases;

public sealed class GetEmployeeRoleByIdUseCase
{
    private readonly IEmployeeRoleRepository _repo;
    public GetEmployeeRoleByIdUseCase(IEmployeeRoleRepository repo) => _repo = repo;

    public async Task<EmployeeRole> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(EmployeeRoleId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"EmployeeRole with id '{id}' was not found.");
        return entity;
    }
}
