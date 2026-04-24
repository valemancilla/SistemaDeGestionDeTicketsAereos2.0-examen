// La aerolínea es el agregado central que representa una empresa de aviación registrada en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.aggregate;

// Agregado Aeroline: encapsula las reglas de negocio de una aerolínea
public class Aeroline
{
    // ID de la aerolínea (Value Object)
    public AirlineId Id { get; private set; }

    // Nombre de la aerolínea (Value Object con validación de formato)
    public AirlineName Name { get; private set; }

    // Código IATA de 2 letras que identifica a la aerolínea mundialmente (Value Object)
    public AirlineIATACode IATACode { get; private set; }

    // ID del país donde está registrada la aerolínea
    public int IdCountry { get; private set; }

    // Indica si la aerolínea está operando actualmente
    public bool Active { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Aeroline(AirlineId id, AirlineName name, AirlineIATACode iataCode, int idCountry, bool active)
    {
        Id = id;
        Name = name;
        IATACode = iataCode;
        IdCountry = idCountry;
        Active = active;
    }

    // Método de fábrica para crear o reconstruir una aerolínea desde la base de datos
    public static Aeroline Create(int id, string name, string iataCode, int idCountry, bool active)
    {
        // Regla: toda aerolínea debe tener un país de origen válido
        if (idCountry <= 0)
            throw new ArgumentException("IdCountry must be greater than 0.", nameof(idCountry));

        // Regla: código IATA y nombre son validados por sus Value Objects
        return new Aeroline(
            AirlineId.Create(id),
            AirlineName.Create(name),
            AirlineIATACode.Create(iataCode),
            idCountry,
            active
        );
    }

    // Método de fábrica para crear una aerolínea nueva (ID = 0, la BD lo asigna después)
    public static Aeroline CreateNew(string name, string iataCode, int idCountry, bool active) => Create(0, name, iataCode, idCountry, active);
}
