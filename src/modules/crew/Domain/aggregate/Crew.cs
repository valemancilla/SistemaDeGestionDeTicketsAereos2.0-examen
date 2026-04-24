// La tripulación agrupa a los empleados que trabajan juntos en un vuelo (pilotos, auxiliares, etc.)
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.aggregate;

// Agregado Crew: encapsula las reglas de negocio de un grupo de tripulación
public class Crew
{
    // ID del grupo de tripulación (Value Object)
    public CrewId Id { get; private set; }

    // Nombre identificador del grupo (ej: "Crew Alpha", "Tripulación 01")
    public CrewGroupName GroupName { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Crew(CrewId id, CrewGroupName groupName)
    {
        Id = id;
        GroupName = groupName;
    }

    // Método de fábrica para crear o reconstruir un grupo de tripulación desde la base de datos
    public static Crew Create(int id, string groupName)
    {
        // Regla: el nombre del grupo es validado por su Value Object (no vacío)
        return new Crew(
            CrewId.Create(id),
            CrewGroupName.Create(groupName)
        );
    }

    // Método de fábrica para crear una tripulación nueva (ID = 0, la BD lo asigna después)
    public static Crew CreateNew(string name) => Create(0, name);
}
