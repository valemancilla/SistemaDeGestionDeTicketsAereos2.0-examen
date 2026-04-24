using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.UseCases;

public sealed class UpdatePaymentUseCase
{
    private readonly IPaymentRepository _repo;
    public UpdatePaymentUseCase(IPaymentRepository repo) => _repo = repo;

    public async Task<Payment> ExecuteAsync(int id, decimal amount, DateTime date, int idBooking, int idPaymentMethod, int idStatus, int? idTicket = null, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(PaymentId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Payment with id '{id}' was not found.");
        var updated = Payment.Create(id, amount, date, idBooking, idPaymentMethod, idStatus, idTicket);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
