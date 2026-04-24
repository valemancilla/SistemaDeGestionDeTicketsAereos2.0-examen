// Caso de uso: actualizar un email de cliente existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Application.UseCases;

public sealed class UpdateCustomerEmailUseCase
{
    private readonly ICustomerEmailRepository _repo;
    public UpdateCustomerEmailUseCase(ICustomerEmailRepository repo) => _repo = repo;

    // Verifica que el email exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<CustomerEmail> ExecuteAsync(int id, string email, int idPerson, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CustomerEmailId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"CustomerEmail with id '{id}' was not found.");
        if (await _repo.IsEmailInUseAsync(email, id, ct))
            throw new InvalidOperationException("Ese correo electrónico ya está registrado; no se puede duplicar en el sistema.");
        var updated = CustomerEmail.Create(id, email, idPerson);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
