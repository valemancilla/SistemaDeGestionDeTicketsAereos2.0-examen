// El pago registra cuánto se pagó por una reserva, de qué forma y cuál es su estado actual
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Entity;

// Entidad que representa la tabla Payment (pago) en la base de datos
public class PaymentEntity
{
    // Clave primaria del pago
    public int IdPayment { get; set; }

    // FK a la reserva a la que corresponde este pago
    public int IdBooking { get; set; }

    // Opcional: tiquete específico asociado al pago (misma reserva)
    public int? IdTicket { get; set; }

    // FK al método de pago utilizado (efectivo, tarjeta, transferencia...)
    public int IdPaymentMethod { get; set; }

    // Monto total pagado
    public decimal Amount { get; set; }

    // Fecha y hora en que se realizó el pago
    public DateTime PaymentDate { get; set; }

    // FK al estado actual del pago (pendiente, aprobado, rechazado...)
    public int IdStatus { get; set; }

    // Navegación a la reserva pagada
    public BookingEntity Booking { get; set; } = null!;

    // Navegación opcional al tiquete pagado
    public TicketEntity? Ticket { get; set; }

    // Navegación al método de pago utilizado
    public PaymentMethodEntity PaymentMethod { get; set; } = null!;

    // Navegación al estado del pago
    public SystemStatusEntity Status { get; set; } = null!;
}
