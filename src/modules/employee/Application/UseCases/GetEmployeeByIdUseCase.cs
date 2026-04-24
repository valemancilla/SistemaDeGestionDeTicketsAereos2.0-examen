// Caso de uso: buscar un empleado por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Application.UseCases;

public sealed class GetEmployeeByIdUseCase
{
    private readonly IEmployeeRepository _repo;
    public GetEmployeeByIdUseCase(IEmployeeRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<Employee> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(EmployeeId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Employee with id '{id}' was not found.");
        return entity;
    }
}
