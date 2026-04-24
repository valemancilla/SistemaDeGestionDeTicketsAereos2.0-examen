// El aeropuerto representa un punto de origen o destino para vuelos, identificado por su código IATA
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.aggregate;

// Agregado Airport: encapsula las reglas de negocio de un aeropuerto
public class Airport
{
    // ID del aeropuerto (Value Object)
    public AirportId Id { get; private set; }

    // Nombre completo del aeropuerto (Value Object con validación)
    public AirportName Name { get; private set; }

    // Código IATA de 3 letras (ej: BOG, MIA, JFK) — Value Object
    public AirportIATACode IATACode { get; private set; }

    // ID de la ciudad donde está ubicado el aeropuerto
    public int IdCity { get; private set; }

    // Indica si el aeropuerto está activo en el sistema
    public bool Active { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Airport(AirportId id, AirportName name, AirportIATACode iataCode, int idCity, bool active)
    {
        Id = id;
        Name = name;
        IATACode = iataCode;
        IdCity = idCity;
        Active = active;
    }

    // Método de fábrica para crear o reconstruir un aeropuerto desde la base de datos
    public static Airport Create(int id, string name, string iataCode, int idCity, bool active)
    {
        // Regla: todo aeropuerto debe pertenecer a una ciudad válida
        if (idCity <= 0)
            throw new ArgumentException("IdCity must be greater than 0.", nameof(idCity));

        // Regla: código IATA y nombre son validados por sus Value Objects
        return new Airport(
            AirportId.Create(id),
            AirportName.Create(name),
            AirportIATACode.Create(iataCode),
            idCity,
            active
        );
    }

    // Método de fábrica para crear un aeropuerto nuevo (ID = 0, la BD lo asigna después)
    public static Airport CreateNew(string name, string iataCode, int idCity, bool active) => Create(0, name, iataCode, idCity, active);
}
