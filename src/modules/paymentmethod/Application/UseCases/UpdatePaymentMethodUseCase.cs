using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Application.UseCases;

public sealed class UpdatePaymentMethodUseCase
{
    private readonly IPaymentMethodRepository _repo;
    public UpdatePaymentMethodUseCase(IPaymentMethodRepository repo) => _repo = repo;

    public async Task<PaymentMethod> ExecuteAsync(int id, string name, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(PaymentMethodId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"PaymentMethod with id '{id}' was not found.");
        var updated = PaymentMethod.Create(id, name);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
