// El rol define qué permisos tiene un usuario dentro del sistema
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Infrastructure.Entity;

// Entidad que representa la tabla Role en la base de datos
// Los roles controlan el acceso al sistema (ej: Administrador, Cliente)
public class RoleEntity
{
    // Clave primaria del rol
    public int IdUserRole { get; set; }

    // Nombre del rol (ej: Administrador, Cliente)
    public string RoleName { get; set; } = string.Empty;

    // Lista de usuarios que tienen asignado este rol
    public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
}
