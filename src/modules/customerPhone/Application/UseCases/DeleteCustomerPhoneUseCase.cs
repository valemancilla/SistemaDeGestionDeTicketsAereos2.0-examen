// Caso de uso: eliminar un teléfono de cliente por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Application.UseCases;

public sealed class DeleteCustomerPhoneUseCase
{
    private readonly ICustomerPhoneRepository _repo;
    public DeleteCustomerPhoneUseCase(ICustomerPhoneRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CustomerPhoneId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(CustomerPhoneId.Create(id), ct);
        return true;
    }
}
