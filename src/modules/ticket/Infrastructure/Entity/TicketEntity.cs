// El ticket es el pasaje individual de un pasajero dentro de una reserva, con su tarifa y estado actual
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;

// Entidad que representa la tabla Ticket (pasaje aéreo) en la base de datos
public class TicketEntity
{
    // Clave primaria del ticket
    public int IdTicket { get; set; }

    // Código único del ticket que se le entrega al pasajero (ej: TK-2024-001)
    public string TicketCode { get; set; } = string.Empty;

    // FK a la reserva a la que pertenece este ticket
    public int IdBooking { get; set; }

    // FK a la tarifa aplicada para este ticket
    public int IdFare { get; set; }

    // FK al estado actual del ticket (emitido, usado, cancelado...)
    public int IdStatus { get; set; }

    // Fecha y hora en que se emitió el ticket
    public DateTime IssueDate { get; set; }

    // Navegación a la reserva del ticket
    public BookingEntity Booking { get; set; } = null!;

    // Navegación a la tarifa aplicada
    public FareEntity Fare { get; set; } = null!;

    // Navegación al estado del ticket
    public SystemStatusEntity Status { get; set; } = null!;

    // Equipajes asociados a este ticket
    public ICollection<BaggageEntity> Baggages { get; set; } = new List<BaggageEntity>();

    // Historial de cambios de estado del ticket
    public ICollection<TicketStatusHistoryEntity> TicketStatusHistories { get; set; } = new List<TicketStatusHistoryEntity>();

    // Check-ins realizados con este ticket
    public ICollection<CheckInEntity> CheckIns { get; set; } = new List<CheckInEntity>();
}
