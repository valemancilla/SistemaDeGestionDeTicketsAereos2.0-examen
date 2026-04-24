// Contrato del repositorio de métodos de pago: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de métodos de pago
public interface IPaymentMethodRepository
{
    // Busca un método de pago por su ID
    Task<PaymentMethod?> GetByIdAsync(PaymentMethodId id, CancellationToken ct = default);

    // Retorna todos los métodos de pago disponibles en el sistema
    Task<IReadOnlyList<PaymentMethod>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo método de pago al sistema
    Task AddAsync(PaymentMethod paymentMethod, CancellationToken ct = default);

    // Actualiza los datos de un método de pago existente
    Task UpdateAsync(PaymentMethod paymentMethod, CancellationToken ct = default);

    // Elimina un método de pago del sistema por su ID
    Task DeleteAsync(PaymentMethodId id, CancellationToken ct = default);
}
