using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.Interfaces;

public interface IPaymentService
{
    Task<Payment> CreateAsync(decimal amount, DateTime date, int idBooking, int idPaymentMethod, int idStatus, int? idTicket = null, CancellationToken cancellationToken = default);

    Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Payment>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Payment> UpdateAsync(int id, decimal amount, DateTime date, int idBooking, int idPaymentMethod, int idStatus, int? idTicket = null, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
