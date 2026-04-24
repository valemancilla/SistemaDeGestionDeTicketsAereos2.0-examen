// Caso de uso: eliminar un cliente por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Application.UseCases;

public sealed class DeleteCustomerUseCase
{
    private readonly ICustomerRepository _repo;
    public DeleteCustomerUseCase(ICustomerRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CustomerId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(CustomerId.Create(id), ct);
        return true;
    }
}
