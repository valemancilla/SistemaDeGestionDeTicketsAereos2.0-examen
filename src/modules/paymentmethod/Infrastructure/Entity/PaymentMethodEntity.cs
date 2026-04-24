// El método de pago indica cómo se pagó una reserva
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Infrastructure.Entity;

// Entidad que representa la tabla PaymentMethod en la base de datos
// Ejemplos: Tarjeta de crédito, PSE, Efectivo
public class PaymentMethodEntity
{
    // Clave primaria del método de pago
    public int IdPaymentMethod { get; set; }

    // Nombre del método (ej: Tarjeta de crédito, PSE, Efectivo)
    public string MethodName { get; set; } = string.Empty;

    // Lista de pagos realizados con este método
    public ICollection<PaymentEntity> Payments { get; set; } = new List<PaymentEntity>();
}
