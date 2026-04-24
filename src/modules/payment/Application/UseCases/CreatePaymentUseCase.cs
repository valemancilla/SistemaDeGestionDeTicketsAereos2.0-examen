using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.UseCases;

public sealed class CreatePaymentUseCase
{
    private readonly IPaymentRepository _repo;
    public CreatePaymentUseCase(IPaymentRepository repo) => _repo = repo;

    public async Task<Payment> ExecuteAsync(decimal amount, DateTime date, int idBooking, int idPaymentMethod, int idStatus, int? idTicket = null, CancellationToken ct = default)
    {
        var entity = Payment.CreateNew(amount, date, idBooking, idPaymentMethod, idStatus, idTicket);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
