// Contrato del repositorio de usuarios: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de usuarios
public interface IUserRepository
{
    // Busca un usuario por su ID
    Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default);

    // Busca un usuario por su nombre de usuario — se usa en el proceso de login para autenticación
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken ct = default);

    // Retorna todos los usuarios del sistema
    Task<IReadOnlyList<User>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo usuario al sistema
    Task AddAsync(User user, CancellationToken ct = default);

    // Actualiza los datos de un usuario existente (ej: cambio de contraseña o rol)
    Task UpdateAsync(User user, CancellationToken ct = default);

    // Elimina un usuario del sistema por su ID
    Task DeleteAsync(UserId id, CancellationToken ct = default);
}
