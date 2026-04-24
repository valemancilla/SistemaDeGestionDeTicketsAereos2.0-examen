// La ruta define el trayecto entre dos aeropuertos con su distancia y duración estimada
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Entity;

// Entidad que representa la tabla Route (ruta entre aeropuertos) en la base de datos
public class RouteEntity
{
    // Clave primaria de la ruta
    public int IdRoute { get; set; }

    // FK al aeropuerto de origen del trayecto
    public int OriginAirport { get; set; }

    // FK al aeropuerto de destino del trayecto
    public int DestinationAirport { get; set; }

    // Navegación al aeropuerto de origen
    public AirportEntity OriginAirportNavigation { get; set; } = null!;

    // Navegación al aeropuerto de destino
    public AirportEntity DestinationAirportNavigation { get; set; } = null!;

    // Distancia en kilómetros entre los dos aeropuertos
    public decimal DistanceKm { get; set; }

    // Duración estimada del vuelo (horas y minutos)
    public TimeOnly EstDuration { get; set; }

    // Indica si esta ruta está activa en el sistema
    public bool Active { get; set; }

    // Vuelos que operan en esta ruta
    public ICollection<FlightEntity> Flights { get; set; } = new List<FlightEntity>();
}
