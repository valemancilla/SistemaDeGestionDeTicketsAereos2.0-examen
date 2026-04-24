using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Application.Interfaces;

public interface IPaymentMethodService
{
    Task<PaymentMethod> CreateAsync(string name, CancellationToken cancellationToken = default);

    Task<PaymentMethod?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PaymentMethod>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PaymentMethod> UpdateAsync(int id, string name, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
