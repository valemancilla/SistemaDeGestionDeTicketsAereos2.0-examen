// Caso de uso: actualizar un teléfono de cliente existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Application.UseCases;

public sealed class UpdateCustomerPhoneUseCase
{
    private readonly ICustomerPhoneRepository _repo;
    public UpdateCustomerPhoneUseCase(ICustomerPhoneRepository repo) => _repo = repo;

    // Verifica que el teléfono exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<CustomerPhone> ExecuteAsync(int id, string phone, int idPerson, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CustomerPhoneId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"CustomerPhone with id '{id}' was not found.");
        if (await _repo.IsPhoneInUseAsync(phone, id, ct))
            throw new InvalidOperationException("Ese número de teléfono ya está registrado; no se puede duplicar en el sistema.");
        var updated = CustomerPhone.Create(id, phone, idPerson);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
