// Caso de uso: actualizar un cliente existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Application.UseCases;

public sealed class UpdateCustomerUseCase
{
    private readonly ICustomerRepository _repo;
    public UpdateCustomerUseCase(ICustomerRepository repo) => _repo = repo;

    // Verifica que el cliente exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<Customer> ExecuteAsync(int id, DateOnly registrationDate, int idPerson, bool active, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CustomerId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Customer with id '{id}' was not found.");
        if (registrationDate <= DateOnly.FromDateTime(DateTime.Today))
            throw new InvalidOperationException("La fecha de registro del cliente debe ser posterior a la fecha de hoy.");
        var updated = Customer.Create(id, registrationDate, idPerson, active);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
