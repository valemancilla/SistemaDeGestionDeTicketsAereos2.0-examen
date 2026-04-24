// El asiento es un puesto físico del avión que tiene un número y una clase (económica, business, etc.)
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;

// Entidad que representa la tabla Seat (asiento del avión) en la base de datos
public class SeatEntity
{
    // Clave primaria del asiento
    public int IdSeat { get; set; }

    // FK al avión al que pertenece este asiento
    public int IdAircraft { get; set; }

    // Número del asiento (ej: 12A, 23B)
    public string Number { get; set; } = string.Empty;

    // FK a la clase del asiento (económica, ejecutiva, primera clase)
    public int IdClase { get; set; }

    // Navegación al avión del asiento
    public AircraftEntity Aircraft { get; set; } = null!;

    // Navegación a la clase del asiento
    public SeatClassEntity SeatClass { get; set; } = null!;

    // Disponibilidad de este asiento en cada vuelo
    public ICollection<SeatFlightEntity> SeatFlights { get; set; } = new List<SeatFlightEntity>();

    // Clientes que tienen asignado este asiento en una reserva
    public ICollection<BookingCustomerEntity> BookingCustomers { get; set; } = new List<BookingCustomerEntity>();

    // Check-ins realizados con este asiento
    public ICollection<CheckInEntity> CheckIns { get; set; } = new List<CheckInEntity>();
}
