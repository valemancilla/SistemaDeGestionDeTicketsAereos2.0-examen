// Cada miembro de tripulación es un empleado con un rol específico dentro de un grupo de tripulación
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Infrastructure.Entity;

// Entidad que representa la tabla CrewMember (miembro de tripulación) en la base de datos
public class CrewMemberEntity
{
    // Clave primaria del miembro de tripulación
    public int IdCrewMember { get; set; }

    // FK a la tripulación a la que pertenece este miembro
    public int IdCrew { get; set; }

    // FK al empleado que actúa como miembro de la tripulación
    public int IdEmployee { get; set; }

    // FK al rol que desempeña dentro de la tripulación (piloto, auxiliar, etc.)
    public int IdRole { get; set; }

    // Navegación a la tripulación
    public CrewEntity Crew { get; set; } = null!;

    // Navegación al empleado
    public EmployeeEntity Employee { get; set; } = null!;

    // Navegación al rol del empleado en la tripulación
    public EmployeeRoleEntity EmployeeRole { get; set; } = null!;
}
