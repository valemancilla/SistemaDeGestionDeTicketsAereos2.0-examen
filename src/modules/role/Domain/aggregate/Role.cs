// El rol de usuario define qué acciones puede realizar en el sistema (admin, cliente, etc.)
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.aggregate;

// Agregado Role: encapsula las reglas de negocio de un rol de usuario del sistema
public class Role
{
    // ID del rol (Value Object)
    public RoleId Id { get; private set; }

    // Nombre del rol (ej: "Administrador", "Cliente", "Agente")
    public RoleName Name { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Role(RoleId id, RoleName name)
    {
        Id = id;
        Name = name;
    }

    // Método de fábrica para crear o reconstruir un rol desde la base de datos
    public static Role Create(int id, string name)
    {
        // Regla: el nombre del rol es validado por su Value Object (no vacío)
        return new Role(
            RoleId.Create(id),
            RoleName.Create(name)
        );
    }

    // Método de fábrica para crear un rol nuevo (ID = 0, la BD lo asigna después)
    public static Role CreateNew(string name) => Create(0, name);
}
