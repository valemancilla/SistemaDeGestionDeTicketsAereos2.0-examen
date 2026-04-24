// La cancelación registra quién canceló la reserva, el motivo y si hubo penalización económica
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Infrastructure.Entity;

// Entidad que representa la tabla BookingCancellation (cancelación de reserva) en la base de datos
public class BookingCancellationEntity
{
    // Clave primaria de la cancelación
    public int IdCancellation { get; set; }

    // FK a la reserva que fue cancelada
    public int IdBooking { get; set; }

    // Motivo por el que se canceló la reserva
    public string CancellationReason { get; set; } = string.Empty;

    // Monto de penalización cobrado por la cancelación (puede ser 0 si no aplica)
    public decimal PenaltyAmount { get; set; }

    // Fecha y hora en que se realizó la cancelación
    public DateTime CancellationDate { get; set; }

    // FK al usuario que procesó la cancelación
    public int IdUser { get; set; }

    // Navegación a la reserva cancelada
    public BookingEntity Booking { get; set; } = null!;

    // Navegación al usuario que realizó la cancelación
    public UserEntity User { get; set; } = null!;
}
