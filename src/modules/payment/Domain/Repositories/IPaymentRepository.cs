// Contrato del repositorio de pagos: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de pagos
public interface IPaymentRepository
{
    // Busca un pago por su ID
    Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken ct = default);

    // Retorna todos los pagos registrados en el sistema
    Task<IReadOnlyList<Payment>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo pago al sistema
    Task AddAsync(Payment payment, CancellationToken ct = default);

    // Actualiza los datos de un pago existente (ej: cambio de estado)
    Task UpdateAsync(Payment payment, CancellationToken ct = default);

    // Elimina un pago del sistema por su ID
    Task DeleteAsync(PaymentId id, CancellationToken ct = default);
}
