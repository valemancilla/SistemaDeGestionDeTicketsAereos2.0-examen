// La zona horaria permite saber la hora local de un aeropuerto — crucial para horarios de vuelo correctos
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.aggregate;

// Renombrado a AirlineTimeZone para evitar conflicto con System.TimeZone de .NET
// Agregado AirlineTimeZone: encapsula las reglas de negocio de una zona horaria del sistema
public class AirlineTimeZone
{
    // ID de la zona horaria (Value Object)
    public TimeZoneId Id { get; private set; }

    // Nombre de la zona horaria (ej: "America/Bogota", "Europe/Madrid")
    public TimeZoneName Name { get; private set; }

    // Desplazamiento respecto a UTC (ej: "-05:00", "+02:00")
    public TimeZoneUTCOffset UTCOffset { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private AirlineTimeZone(TimeZoneId id, TimeZoneName name, TimeZoneUTCOffset utcOffset)
    {
        Id = id;
        Name = name;
        UTCOffset = utcOffset;
    }

    // Método de fábrica para crear o reconstruir una zona horaria desde la base de datos
    public static AirlineTimeZone Create(int id, string name, string utcOffset)
    {
        // Regla: nombre y offset UTC son validados por sus respectivos Value Objects
        return new AirlineTimeZone(
            TimeZoneId.Create(id),
            TimeZoneName.Create(name),
            TimeZoneUTCOffset.Create(utcOffset)
        );
    }

    // Método de fábrica para crear una zona horaria nueva (ID = 0, la BD lo asigna después)
    public static AirlineTimeZone CreateNew(string name, string utcOffset) => Create(0, name, utcOffset);
}
