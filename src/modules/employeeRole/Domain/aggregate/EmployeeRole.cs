// El rol del empleado define qué función cumple dentro de la aerolínea (piloto, copiloto, agente de tierra, etc.)
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.aggregate;

// Agregado EmployeeRole: encapsula las reglas de negocio de un rol de empleado
public class EmployeeRole
{
    // ID del rol (Value Object)
    public EmployeeRoleId Id { get; private set; }

    // Nombre del rol (ej: "Piloto", "Auxiliar de Vuelo", "Agente de Mostrador")
    public EmployeeRoleName Name { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private EmployeeRole(EmployeeRoleId id, EmployeeRoleName name)
    {
        Id = id;
        Name = name;
    }

    // Método de fábrica para crear o reconstruir un rol de empleado desde la base de datos
    public static EmployeeRole Create(int id, string name)
    {
        // Regla: el nombre del rol es validado por su Value Object (no vacío)
        return new EmployeeRole(
            EmployeeRoleId.Create(id),
            EmployeeRoleName.Create(name)
        );
    }

    // Método de fábrica para crear un rol nuevo (ID = 0, la BD lo asigna después)
    public static EmployeeRole CreateNew(string name) => Create(0, name);
}
