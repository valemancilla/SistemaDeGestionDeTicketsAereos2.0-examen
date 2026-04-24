// Guarda el historial de cambios de estado de una reserva para saber quién cambió qué y cuándo
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Infrastructure.Entity;

// Entidad que representa la tabla BookingStatusHistory (historial de estados de una reserva)
public class BookingStatusHistoryEntity
{
    // Clave primaria del registro histórico
    public int IdHistory { get; set; }

    // FK a la reserva cuyo estado cambió
    public int IdBooking { get; set; }

    // FK al nuevo estado que tomó la reserva
    public int IdStatus { get; set; }

    // Fecha y hora en que se hizo el cambio de estado
    public DateTime ChangeDate { get; set; }

    // FK al usuario que realizó el cambio
    public int IdUser { get; set; }

    // Observación opcional sobre el motivo del cambio
    public string? Observation { get; set; }

    // Navegación a la reserva
    public BookingEntity Booking { get; set; } = null!;

    // Navegación al estado registrado
    public SystemStatusEntity Status { get; set; } = null!;

    // Navegación al usuario que hizo el cambio
    public UserEntity User { get; set; } = null!;
}
