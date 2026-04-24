using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Application.UseCases;

public sealed class GetAllPaymentMethodsUseCase
{
    private readonly IPaymentMethodRepository _repo;
    public GetAllPaymentMethodsUseCase(IPaymentMethodRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<PaymentMethod>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
