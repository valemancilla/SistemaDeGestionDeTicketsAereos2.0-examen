// El empleado trabaja para una aerolínea, tiene un rol asignado y puede pertenecer a tripulaciones
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Entity;

// Entidad que representa la tabla Employee (empleado) en la base de datos
public class EmployeeEntity
{
    // Clave primaria del empleado
    public int IdEmployee { get; set; }

    // FK a la persona con los datos personales del empleado
    public int IdPerson { get; set; }

    // FK a la aerolínea para la que trabaja el empleado
    public int IdAirline { get; set; }

    // FK al rol del empleado dentro de la aerolínea (ej: piloto, técnico, administrador)
    public int IdRole { get; set; }

    // Navegación a los datos personales del empleado
    public PersonEntity Person { get; set; } = null!;

    // Navegación a la aerolínea empleadora
    public AerolineEntity Airline { get; set; } = null!;

    // Navegación al rol del empleado
    public EmployeeRoleEntity Role { get; set; } = null!;

    // Tripulaciones de vuelo en las que participa este empleado
    public ICollection<CrewMemberEntity> CrewMembers { get; set; } = new List<CrewMemberEntity>();
}
