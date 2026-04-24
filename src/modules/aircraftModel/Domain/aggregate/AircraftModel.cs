// El modelo de aeronave representa el tipo de avión (ej: Boeing 737) fabricado por un fabricante específico
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.aggregate;

// Agregado AircraftModel: encapsula las reglas de negocio de un modelo de aeronave
public class AircraftModel
{
    // ID del modelo (Value Object)
    public AircraftModelId Id { get; private set; }

    // Nombre del modelo (ej: Boeing 737, Airbus A320) con validación
    public AircraftModelName Name { get; private set; }

    // ID del fabricante que produce este modelo
    public int IdManufacturer { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private AircraftModel(AircraftModelId id, AircraftModelName name, int idManufacturer)
    {
        Id = id;
        Name = name;
        IdManufacturer = idManufacturer;
    }

    // Método de fábrica para crear o reconstruir un modelo desde la base de datos
    public static AircraftModel Create(int id, string name, int idManufacturer)
    {
        // Regla: todo modelo de aeronave debe tener un fabricante válido asociado
        if (idManufacturer <= 0)
            throw new ArgumentException("IdManufacturer must be greater than 0.", nameof(idManufacturer));

        // Regla: el nombre del modelo es validado por su Value Object (no vacío)
        return new AircraftModel(
            AircraftModelId.Create(id),
            AircraftModelName.Create(name),
            idManufacturer
        );
    }

    // Método de fábrica para crear un modelo nuevo (ID = 0, la BD lo asigna después)
    public static AircraftModel CreateNew(string name, int idManufacturer) => Create(0, name, idManufacturer);
}
