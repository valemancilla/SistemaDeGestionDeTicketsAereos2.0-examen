// El género representa las opciones de género registradas en el sistema para clasificar a las personas
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.aggregate;

// Agregado Gender: encapsula las reglas de negocio de un género en el sistema
public class Gender
{
    // ID del género (Value Object)
    public GenderId Id { get; private set; }

    // Descripción del género (ej: Masculino, Femenino, No binario)
    public GenderDescription Description { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Gender(GenderId id, GenderDescription description)
    {
        Id = id;
        Description = description;
    }

    // Método de fábrica para crear o reconstruir un género
    public static Gender Create(int id, string description)
    {
        // Regla: la descripción es validada por su Value Object (no vacía, formato válido)
        return new Gender(
            GenderId.Create(id),
            GenderDescription.Create(description)
        );
    }

    // Método de fábrica para crear un género nuevo (ID = 0, la BD lo asigna después)
    public static Gender CreateNew(string name) => Create(0, name);
}
