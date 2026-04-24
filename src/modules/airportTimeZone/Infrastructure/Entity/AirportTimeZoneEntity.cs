// Esta tabla relaciona aeropuertos con zonas horarias, un aeropuerto puede manejar varias zonas horarias
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Infrastructure.Entity;

// Entidad que representa la tabla AirportTimeZone (relación entre aeropuerto y zona horaria)
public class AirportTimeZoneEntity
{
    // Parte de la clave primaria compuesta, FK al aeropuerto
    public int IdAirport { get; set; }

    // Parte de la clave primaria compuesta, FK a la zona horaria
    public int IdTimeZone { get; set; }

    // Navegación al aeropuerto
    public AirportEntity Airport { get; set; } = null!;

    // Navegación a la zona horaria
    public TimeZoneEntity TimeZone { get; set; } = null!;
}
