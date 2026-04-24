// Caso de uso: obtener todos los empleados registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Application.UseCases;

public sealed class GetAllEmployeesUseCase
{
    private readonly IEmployeeRepository _repo;
    public GetAllEmployeesUseCase(IEmployeeRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<Employee>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
