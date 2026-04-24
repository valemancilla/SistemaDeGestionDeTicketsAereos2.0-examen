using SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Payment> CreateAsync(decimal amount, DateTime date, int idBooking, int idPaymentMethod, int idStatus, int? idTicket = null, CancellationToken cancellationToken = default)
    {
        var entity = Payment.CreateNew(amount, date, idBooking, idPaymentMethod, idStatus, idTicket);
        await _paymentRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _paymentRepository.GetByIdAsync(PaymentId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Payment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _paymentRepository.ListAsync(cancellationToken);
    }

    public async Task<Payment> UpdateAsync(int id, decimal amount, DateTime date, int idBooking, int idPaymentMethod, int idStatus, int? idTicket = null, CancellationToken cancellationToken = default)
    {
        var paymentId = PaymentId.Create(id);
        var existing = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Payment with id '{id}' was not found.");

        var updated = Payment.Create(id, amount, date, idBooking, idPaymentMethod, idStatus, idTicket);
        await _paymentRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var paymentId = PaymentId.Create(id);
        var existing = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (existing is null)
            return false;

        await _paymentRepository.DeleteAsync(paymentId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
