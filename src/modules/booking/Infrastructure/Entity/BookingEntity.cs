// La reserva agrupa tickets, pagos y clientes para un vuelo específico, y lleva un historial de estados
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;

// Entidad que representa la tabla Booking (reserva) en la base de datos
public class BookingEntity
{
    // Clave primaria de la reserva
    public int IdBooking { get; set; }

    // Código único de la reserva que se le da al pasajero (ej: ABC123)
    public string BookingCode { get; set; } = string.Empty;

    // Fecha y hora del vuelo reservado
    public DateTime FlightDate { get; set; }

    // FK al estado actual de la reserva (pendiente, confirmada, cancelada...)
    public int IdStatus { get; set; }

    // FK al vuelo reservado
    public int IdFlight { get; set; }

    // Cantidad de asientos reservados
    public int SeatCount { get; set; }

    // Fecha en que se creó la reserva
    public DateOnly CreationDate { get; set; }

    // Observaciones opcionales sobre la reserva
    public string? Observations { get; set; }

    // Contacto y consentimientos del titular (cliente)
    public string? HolderEmail { get; set; }

    public string? HolderPhonePrefix { get; set; }

    public string? HolderPhone { get; set; }

    public bool ConsentDataProcessing { get; set; }

    public bool ConsentMarketing { get; set; }

    public int? IdHolderPerson { get; set; }

    public PersonEntity? HolderPerson { get; set; }

    // Navegación al estado actual de la reserva
    public SystemStatusEntity Status { get; set; } = null!;

    // Navegación al vuelo reservado
    public FlightEntity Flight { get; set; } = null!;

    // Tickets (pasajes) que forman parte de esta reserva
    public ICollection<TicketEntity> Tickets { get; set; } = new List<TicketEntity>();

    // Clientes que están incluidos en esta reserva
    public ICollection<BookingCustomerEntity> BookingCustomers { get; set; } = new List<BookingCustomerEntity>();

    // Pagos realizados para esta reserva
    public ICollection<PaymentEntity> Payments { get; set; } = new List<PaymentEntity>();

    // Cancelaciones que se hayan hecho sobre esta reserva
    public ICollection<BookingCancellationEntity> BookingCancellations { get; set; } = new List<BookingCancellationEntity>();

    // Historial de cambios de estado de la reserva
    public ICollection<BookingStatusHistoryEntity> BookingStatusHistories { get; set; } = new List<BookingStatusHistoryEntity>();
}
