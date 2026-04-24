// Caso de uso: actualizar un empleado existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Application.UseCases;

public sealed class UpdateEmployeeUseCase
{
    private readonly IEmployeeRepository _repo;
    public UpdateEmployeeUseCase(IEmployeeRepository repo) => _repo = repo;

    // Verifica que el empleado exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<Employee> ExecuteAsync(int id, int idPerson, int idAirline, int idRole, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(EmployeeId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Employee with id '{id}' was not found.");
        var updated = Employee.Create(id, idPerson, idAirline, idRole);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
