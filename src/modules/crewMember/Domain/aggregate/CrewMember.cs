// Un miembro de tripulación es la asignación de un empleado a un grupo con un rol específico dentro del vuelo
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.aggregate;

// Agregado CrewMember: encapsula la relación entre un empleado, su tripulación y su rol
public class CrewMember
{
    // ID del miembro de tripulación (Value Object)
    public CrewMemberId Id { get; private set; }

    // FK al grupo de tripulación al que pertenece
    public int IdCrew { get; private set; }

    // FK al empleado que forma parte de la tripulación
    public int IdEmployee { get; private set; }

    // FK al rol que desempeña dentro del grupo (piloto, copiloto, auxiliar, etc.)
    public int IdRole { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private CrewMember(CrewMemberId id, int idCrew, int idEmployee, int idRole)
    {
        Id = id;
        IdCrew = idCrew;
        IdEmployee = idEmployee;
        IdRole = idRole;
    }

    // Método de fábrica para crear o reconstruir un miembro de tripulación desde la base de datos
    public static CrewMember Create(int id, int idCrew, int idEmployee, int idRole)
    {
        // Regla: el miembro debe pertenecer a un grupo de tripulación válido
        if (idCrew <= 0)
            throw new ArgumentException("IdCrew must be greater than 0.", nameof(idCrew));

        // Regla: el miembro debe ser un empleado válido registrado en el sistema
        if (idEmployee <= 0)
            throw new ArgumentException("IdEmployee must be greater than 0.", nameof(idEmployee));

        // Regla: el miembro debe tener un rol definido dentro de la tripulación
        if (idRole <= 0)
            throw new ArgumentException("IdRole must be greater than 0.", nameof(idRole));

        return new CrewMember(
            CrewMemberId.Create(id),
            idCrew,
            idEmployee,
            idRole
        );
    }

    // Método de fábrica para crear un miembro nuevo (ID = 0, la BD lo asigna después)
    public static CrewMember CreateNew(int idCrew, int idEmployee, int idRole) => Create(0, idCrew, idEmployee, idRole);
}
