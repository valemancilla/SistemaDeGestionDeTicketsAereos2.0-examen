// Esta tabla relaciona a los clientes con una reserva específica, indicando qué asiento tiene cada uno
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Entity;

// Entidad que representa la tabla BookingCustomer (cliente dentro de una reserva)
public class BookingCustomerEntity
{
    // Clave primaria del registro cliente-reserva
    public int IdBookingCustomer { get; set; }

    // FK a la reserva a la que pertenece este cliente
    public int IdBooking { get; set; }

    // FK al usuario del sistema que es el cliente
    public int IdUser { get; set; }

    // FK a la persona (datos personales del pasajero)
    public int IdPerson { get; set; }

    // FK al asiento asignado dentro del avión
    public int IdSeat { get; set; }

    // Indica si este cliente es el titular principal de la reserva
    public bool IsPrimary { get; set; }

    // Fecha y hora en que se asoció el cliente a la reserva
    public DateTime AssociationDate { get; set; }

    // Navegación a la reserva
    public BookingEntity Booking { get; set; } = null!;

    // Navegación al usuario
    public UserEntity User { get; set; } = null!;

    // Navegación a la persona con sus datos personales
    public PersonEntity Person { get; set; } = null!;

    // Navegación al asiento asignado
    public SeatEntity Seat { get; set; } = null!;
}
