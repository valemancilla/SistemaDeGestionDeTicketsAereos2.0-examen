using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Application.Services;

public sealed class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, IUnitOfWork unitOfWork)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentMethod> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = PaymentMethod.CreateNew(name);
        await _paymentMethodRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<PaymentMethod?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _paymentMethodRepository.GetByIdAsync(PaymentMethodId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<PaymentMethod>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _paymentMethodRepository.ListAsync(cancellationToken);
    }

    public async Task<PaymentMethod> UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        var paymentMethodId = PaymentMethodId.Create(id);
        var existing = await _paymentMethodRepository.GetByIdAsync(paymentMethodId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"PaymentMethod with id '{id}' was not found.");

        var updated = PaymentMethod.Create(id, name);
        await _paymentMethodRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var paymentMethodId = PaymentMethodId.Create(id);
        var existing = await _paymentMethodRepository.GetByIdAsync(paymentMethodId, cancellationToken);
        if (existing is null)
            return false;

        await _paymentMethodRepository.DeleteAsync(paymentMethodId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
