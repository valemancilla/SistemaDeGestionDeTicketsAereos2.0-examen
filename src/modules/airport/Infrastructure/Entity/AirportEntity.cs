// El aeropuerto pertenece a una ciudad y puede ser origen o destino de rutas, además tiene zonas horarias asociadas
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Entity;

// Entidad que representa la tabla Airport (aeropuerto) en la base de datos
public class AirportEntity
{
    // Clave primaria del aeropuerto
    public int IdAirport { get; set; }

    // Nombre completo del aeropuerto (ej: El Dorado International Airport)
    public string Name { get; set; } = string.Empty;

    // Código IATA de 3 letras que identifica al aeropuerto mundialmente (ej: BOG, MIA)
    public string IATACode { get; set; } = string.Empty;

    // FK a la ciudad donde está ubicado el aeropuerto
    public int IdCity { get; set; }

    // Indica si el aeropuerto está activo en el sistema
    public bool Active { get; set; }

    // Navegación a la ciudad del aeropuerto
    public CityEntity City { get; set; } = null!;

    // Rutas donde este aeropuerto es el punto de salida
    public ICollection<RouteEntity> RoutesAsOrigin { get; set; } = new List<RouteEntity>();

    // Rutas donde este aeropuerto es el punto de llegada
    public ICollection<RouteEntity> RoutesAsDestination { get; set; } = new List<RouteEntity>();

    // Zonas horarias que aplican a este aeropuerto
    public ICollection<AirportTimeZoneEntity> AirportTimeZones { get; set; } = new List<AirportTimeZoneEntity>();
}
