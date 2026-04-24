// El rol de empleado define qué función cumple dentro de la aerolínea
// No confundir con el rol de usuario del sistema (RoleEntity)
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Infrastructure.Entity;

// Entidad que representa la tabla EmployeeRole en la base de datos
// Ejemplos: Piloto, Copiloto, Auxiliar de vuelo, Técnico
public class EmployeeRoleEntity
{
    // Clave primaria del rol de empleado
    public int IdRole { get; set; }

    // Nombre del rol (ej: Piloto, Auxiliar de vuelo)
    public string RoleName { get; set; } = string.Empty;

    // Empleados que tienen este rol
    public ICollection<EmployeeEntity> Employees { get; set; } = new List<EmployeeEntity>();

    // Miembros de tripulación que tienen este rol
    public ICollection<CrewMemberEntity> CrewMembers { get; set; } = new List<CrewMemberEntity>();
}
