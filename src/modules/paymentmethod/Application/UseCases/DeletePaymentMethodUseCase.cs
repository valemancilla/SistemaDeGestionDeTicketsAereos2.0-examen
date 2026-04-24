using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Application.UseCases;

public sealed class DeletePaymentMethodUseCase
{
    private readonly IPaymentMethodRepository _repo;
    public DeletePaymentMethodUseCase(IPaymentMethodRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(PaymentMethodId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(PaymentMethodId.Create(id), ct);
        return true;
    }
}
