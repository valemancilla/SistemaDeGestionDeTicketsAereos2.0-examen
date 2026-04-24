// Caso de uso: eliminar un email de cliente por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Application.UseCases;

public sealed class DeleteCustomerEmailUseCase
{
    private readonly ICustomerEmailRepository _repo;
    public DeleteCustomerEmailUseCase(ICustomerEmailRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CustomerEmailId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(CustomerEmailId.Create(id), ct);
        return true;
    }
}
