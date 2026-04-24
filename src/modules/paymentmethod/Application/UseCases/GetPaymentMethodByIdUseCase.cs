using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Application.UseCases;

public sealed class GetPaymentMethodByIdUseCase
{
    private readonly IPaymentMethodRepository _repo;
    public GetPaymentMethodByIdUseCase(IPaymentMethodRepository repo) => _repo = repo;

    public async Task<PaymentMethod> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(PaymentMethodId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"PaymentMethod with id '{id}' was not found.");
        return entity;
    }
}
