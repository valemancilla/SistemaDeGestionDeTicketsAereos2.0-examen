// El tipo de equipaje categoriza el equipaje del pasajero (ej: de mano, bodega, deportivo)
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.aggregate;

// Agregado BaggageType: encapsula las reglas de negocio de un tipo de equipaje
public class BaggageType
{
    // ID del tipo de equipaje (Value Object)
    public BaggageTypeId Id { get; private set; }

    // Nombre del tipo de equipaje (Value Object con validación)
    public BaggageTypeName Name { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private BaggageType(BaggageTypeId id, BaggageTypeName name)
    {
        Id = id;
        Name = name;
    }

    // Método de fábrica para crear o reconstruir un tipo de equipaje
    public static BaggageType Create(int id, string name)
    {
        // Regla: el nombre del tipo de equipaje es validado por su Value Object (no vacío)
        return new BaggageType(
            BaggageTypeId.Create(id),
            BaggageTypeName.Create(name)
        );
    }

    // Método de fábrica para crear un tipo nuevo (ID = 0, la BD lo asigna después)
    public static BaggageType CreateNew(string name) => Create(0, name);
}
