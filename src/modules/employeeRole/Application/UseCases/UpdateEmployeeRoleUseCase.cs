using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Application.UseCases;

public sealed class UpdateEmployeeRoleUseCase
{
    private readonly IEmployeeRoleRepository _repo;
    public UpdateEmployeeRoleUseCase(IEmployeeRoleRepository repo) => _repo = repo;

    public async Task<EmployeeRole> ExecuteAsync(int id, string name, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(EmployeeRoleId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"EmployeeRole with id '{id}' was not found.");
        var updated = EmployeeRole.Create(id, name);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
