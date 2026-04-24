// El avión pertenece a una aerolínea, tiene un modelo específico y puede tener asientos y vuelos asignados
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;

// Entidad que representa la tabla Aircraft (avión) en la base de datos
public class AircraftEntity
{
    // Clave primaria del avión
    public int IdAircraft { get; set; }

    // FK a la aerolínea dueña del avión
    public int IdAirline { get; set; }

    // FK al modelo del avión (ej: Boeing 737)
    public int IdModel { get; set; }

    // Capacidad total de pasajeros del avión
    public int Capacity { get; set; }

    // Navegación a la aerolínea
    public AerolineEntity Airline { get; set; } = null!;

    // Navegación al modelo de aeronave
    public AircraftModelEntity Model { get; set; } = null!;

    // Asientos físicos que tiene este avión
    public ICollection<SeatEntity> Seats { get; set; } = new List<SeatEntity>();

    // Vuelos en los que opera este avión
    public ICollection<FlightEntity> Flights { get; set; } = new List<FlightEntity>();
}
