// El fabricante es la empresa que construye las aeronaves (Boeing, Airbus, Embraer, etc.)
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.aggregate;

// Agregado Manufacturer: encapsula las reglas de negocio de un fabricante de aeronaves
public class Manufacturer
{
    // ID del fabricante (Value Object)
    public ManufacturerId Id { get; private set; }

    // Nombre del fabricante (ej: "Boeing", "Airbus", "Embraer")
    public ManufacturerName Name { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Manufacturer(ManufacturerId id, ManufacturerName name)
    {
        Id = id;
        Name = name;
    }

    // Método de fábrica para crear o reconstruir un fabricante desde la base de datos
    public static Manufacturer Create(int id, string name)
    {
        // Regla: el nombre del fabricante es validado por su Value Object (no vacío)
        return new Manufacturer(
            ManufacturerId.Create(id),
            ManufacturerName.Create(name)
        );
    }

    // Método de fábrica para crear un fabricante nuevo (ID = 0, la BD lo asigna después)
    public static Manufacturer CreateNew(string name) => Create(0, name);
}
