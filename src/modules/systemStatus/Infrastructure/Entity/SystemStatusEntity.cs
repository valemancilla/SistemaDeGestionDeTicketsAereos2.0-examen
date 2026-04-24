// El estado del sistema es compartido por varias entidades: vuelos, reservas, tiquetes, pagos, etc.
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;

// Entidad que representa la tabla SystemStatus en la base de datos
// Un estado puede ser: Activo, Cancelado, Pendiente, etc.
// El campo EntityType indica a qué tipo de entidad aplica ese estado (Vuelo, Reserva, Tiquete...)
public class SystemStatusEntity
{
    // Clave primaria del estado
    public int IdStatus { get; set; }

    // Nombre del estado (ej: Activo, Cancelado, Pendiente)
    public string StatusName { get; set; } = string.Empty;

    // Tipo de entidad a la que aplica este estado (ej: Vuelo, Reserva, Tiquete)
    public string EntityType { get; set; } = string.Empty;

    // Reservas que tienen este estado
    public ICollection<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();

    // Vuelos que tienen este estado
    public ICollection<FlightEntity> Flights { get; set; } = new List<FlightEntity>();

    // Tiquetes que tienen este estado
    public ICollection<TicketEntity> Tickets { get; set; } = new List<TicketEntity>();

    // Pagos que tienen este estado
    public ICollection<PaymentEntity> Payments { get; set; } = new List<PaymentEntity>();

    // Check-ins que tienen este estado
    public ICollection<CheckInEntity> CheckIns { get; set; } = new List<CheckInEntity>();

    // Historial de cambios de estado de vuelos
    public ICollection<FlightStatusHistoryEntity> FlightStatusHistories { get; set; } = new List<FlightStatusHistoryEntity>();

    // Historial de cambios de estado de reservas
    public ICollection<BookingStatusHistoryEntity> BookingStatusHistories { get; set; } = new List<BookingStatusHistoryEntity>();

    // Historial de cambios de estado de tiquetes
    public ICollection<TicketStatusHistoryEntity> TicketStatusHistories { get; set; } = new List<TicketStatusHistoryEntity>();
}
