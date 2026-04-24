using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.UseCases;

public sealed class GetPaymentByIdUseCase
{
    private readonly IPaymentRepository _repo;
    public GetPaymentByIdUseCase(IPaymentRepository repo) => _repo = repo;

    public async Task<Payment> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(PaymentId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Payment with id '{id}' was not found.");
        return entity;
    }
}
