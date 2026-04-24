// Caso de uso: eliminar un empleado por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Application.UseCases;

public sealed class DeleteEmployeeUseCase
{
    private readonly IEmployeeRepository _repo;
    public DeleteEmployeeUseCase(IEmployeeRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(EmployeeId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(EmployeeId.Create(id), ct);
        return true;
    }
}
