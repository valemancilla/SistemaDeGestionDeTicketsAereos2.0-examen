using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Application.UseCases;

public sealed class CreatePaymentMethodUseCase
{
    private readonly IPaymentMethodRepository _repo;
    public CreatePaymentMethodUseCase(IPaymentMethodRepository repo) => _repo = repo;

    public async Task<PaymentMethod> ExecuteAsync(string name, CancellationToken ct = default)
    {
        var entity = PaymentMethod.CreateNew(name);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
