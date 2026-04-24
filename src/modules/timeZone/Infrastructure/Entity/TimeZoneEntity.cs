// La zona horaria se asocia a los aeropuertos para saber en qué huso horario operan
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Infrastructure.Entity;

// Entidad que representa la tabla TimeZone en la base de datos
// Ejemplos: America/Bogota (UTC-5), America/New_York (UTC-5/-4)
public class TimeZoneEntity
{
    // Clave primaria de la zona horaria
    public int IdTimeZone { get; set; }

    // Nombre descriptivo de la zona (ej: America/Bogota, Europe/Madrid)
    public string Name { get; set; } = string.Empty;

    // Diferencia con UTC (ej: -05:00, +01:00)
    public string UTCOffset { get; set; } = string.Empty;

    // Aeropuertos que operan en esta zona horaria (tabla intermedia)
    public ICollection<AirportTimeZoneEntity> AirportTimeZones { get; set; } = new List<AirportTimeZoneEntity>();
}
