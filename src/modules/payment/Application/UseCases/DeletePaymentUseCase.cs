using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.UseCases;

public sealed class DeletePaymentUseCase
{
    private readonly IPaymentRepository _repo;
    public DeletePaymentUseCase(IPaymentRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(PaymentId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(PaymentId.Create(id), ct);
        return true;
    }
}
