namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.aggregate;

// Agregado AirportTimeZone: representa la relación entre un aeropuerto y su zona horaria
// No tiene Value Objects propios porque su identidad es la combinación de los dos IDs (clave compuesta)
public class AirportTimeZone
{
    // ID del aeropuerto (parte de la clave compuesta)
    public int IdAirport { get; private set; }

    // ID de la zona horaria (parte de la clave compuesta)
    public int IdTimeZone { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private AirportTimeZone(int idAirport, int idTimeZone)
    {
        IdAirport = idAirport;
        IdTimeZone = idTimeZone;
    }

    // Método de fábrica que valida que ambos IDs sean referencias válidas
    public static AirportTimeZone Create(int idAirport, int idTimeZone)
    {
        // Regla: el aeropuerto asociado debe ser una referencia válida
        if (idAirport <= 0)
            throw new ArgumentException("IdAirport must be greater than 0.", nameof(idAirport));

        // Regla: la zona horaria asociada debe ser una referencia válida
        if (idTimeZone <= 0)
            throw new ArgumentException("IdTimeZone must be greater than 0.", nameof(idTimeZone));

        return new AirportTimeZone(idAirport, idTimeZone);
    }
}
