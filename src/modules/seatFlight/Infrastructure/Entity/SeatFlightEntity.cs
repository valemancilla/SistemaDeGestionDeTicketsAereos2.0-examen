// Relaciona un asiento del avión con un vuelo específico para saber si está disponible o no en ese vuelo
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Entity;

// Entidad que representa la tabla SeatFlight (disponibilidad de asiento en un vuelo)
public class SeatFlightEntity
{
    // Clave primaria del registro
    public int IdSeatFlight { get; set; }

    // FK al asiento físico del avión
    public int IdSeat { get; set; }

    // FK al vuelo en el que aplica esta disponibilidad
    public int IdFlight { get; set; }

    // Indica si el asiento está disponible para ser reservado en este vuelo
    public bool Available { get; set; }

    // Navegación al asiento
    public SeatEntity Seat { get; set; } = null!;

    // Navegación al vuelo
    public FlightEntity Flight { get; set; } = null!;
}
