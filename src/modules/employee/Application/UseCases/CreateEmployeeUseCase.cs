// Caso de uso: registrar un nuevo empleado verificando que la persona no tenga ya un vínculo laboral
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Application.UseCases;

public sealed class CreateEmployeeUseCase
{
    private readonly IEmployeeRepository _repo;
    public CreateEmployeeUseCase(IEmployeeRepository repo) => _repo = repo;

    // Una persona no puede tener dos registros de empleado — se busca por idPerson antes de crear
    public async Task<Employee> ExecuteAsync(int idPerson, int idAirline, int idRole, CancellationToken ct = default)
    {
        var existing = await _repo.GetByPersonIdAsync(idPerson, ct);
        if (existing is not null) throw new InvalidOperationException($"Employee for person '{idPerson}' already exists.");
        var entity = Employee.CreateNew(idPerson, idAirline, idRole);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
