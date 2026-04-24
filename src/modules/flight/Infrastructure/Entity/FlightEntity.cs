// El vuelo es la instancia concreta de una ruta en una fecha específica, con avión y tripulación asignados
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;

// Entidad que representa la tabla Flight (vuelo) en la base de datos
public class FlightEntity
{
    // Clave primaria del vuelo
    public int IdFlight { get; set; }

    // FK a la ruta que sigue este vuelo (origen-destino)
    public int IdRoute { get; set; }

    // FK al avión que opera este vuelo
    public int IdAircraft { get; set; }

    // Número de vuelo (ej: AV101, LA231)
    public string FlightNumber { get; set; } = string.Empty;

    // Fecha del vuelo
    public DateOnly Date { get; set; }

    // Hora de salida del vuelo
    public TimeOnly DepartureTime { get; set; }

    // Hora de llegada estimada
    public TimeOnly ArrivalTime { get; set; }

    // Capacidad total de pasajeros en este vuelo
    public int TotalCapacity { get; set; }

    // Cuántos asientos quedan disponibles para reservar
    public int AvailableSeats { get; set; }

    // FK al estado actual del vuelo (programado, en vuelo, cancelado...)
    public int IdStatus { get; set; }

    // FK a la tripulación que opera este vuelo
    public int IdCrew { get; set; }

    // FK a la tarifa asignada a este vuelo (precio del vuelo).
    // Null = vuelo sin precio asignado (compatibilidad con datos antiguos).
    public int? IdFare { get; set; }

    // Navegación a la ruta del vuelo
    public RouteEntity Route { get; set; } = null!;

    // Navegación al avión asignado
    public AircraftEntity Aircraft { get; set; } = null!;

    // Navegación al estado actual del vuelo
    public SystemStatusEntity Status { get; set; } = null!;

    // Navegación a la tripulación del vuelo
    public CrewEntity Crew { get; set; } = null!;

    // Navegación a la tarifa asignada al vuelo
    public FareEntity? Fare { get; set; }

    // Reservas asociadas a este vuelo
    public ICollection<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();

    // Asientos disponibles para este vuelo específico
    public ICollection<SeatFlightEntity> SeatFlights { get; set; } = new List<SeatFlightEntity>();

    // Historial de cambios de estado del vuelo
    public ICollection<FlightStatusHistoryEntity> FlightStatusHistories { get; set; } = new List<FlightStatusHistoryEntity>();
}
