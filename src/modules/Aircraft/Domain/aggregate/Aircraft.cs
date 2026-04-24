// El avión es el agregado que representa una aeronave registrada con su capacidad, aerolínea y modelo
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.aggregate;

// Agregado Aircraft: encapsula las reglas de negocio de un avión en el sistema
public class Aircraft
{
    // ID del avión (Value Object)
    public AircraftId Id { get; private set; }

    // Capacidad de pasajeros del avión (Value Object con validación de rango)
    public AircraftCapacity Capacity { get; private set; }

    // ID de la aerolínea dueña del avión
    public int IdAirline { get; private set; }

    // ID del modelo de aeronave (ej: Boeing 737)
    public int IdModel { get; private set; }

    // Constructor privado: solo se puede crear un Aircraft usando el método Create
    private Aircraft(AircraftId id, AircraftCapacity capacity, int idAirline, int idModel)
    {
        Id = id;
        Capacity = capacity;
        IdAirline = idAirline;
        IdModel = idModel;
    }

    // Método de fábrica para crear o reconstruir un avión desde la base de datos
    public static Aircraft Create(int id, int capacity, int idAirline, int idModel)
    {
        // Regla: toda aeronave debe pertenecer a una aerolínea válida
        if (idAirline <= 0)
            throw new ArgumentException("IdAirline must be greater than 0.", nameof(idAirline));

        // Regla: toda aeronave debe tener un modelo asignado válido
        if (idModel <= 0)
            throw new ArgumentException("IdModel must be greater than 0.", nameof(idModel));

        // Regla: la capacidad debe ser mayor a cero (validada por su Value Object)
        return new Aircraft(
            AircraftId.Create(id),
            AircraftCapacity.Create(capacity),
            idAirline,
            idModel
        );
    }

    // Método de fábrica para crear un avión nuevo (ID = 0, la BD lo asigna después)
    public static Aircraft CreateNew(int capacity, int idAirline, int idModel) => Create(0, capacity, idAirline, idModel);
}
