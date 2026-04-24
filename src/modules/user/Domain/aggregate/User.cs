// El usuario representa a cualquier persona que puede iniciar sesión en el sistema con un rol específico
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.aggregate;

// Agregado User: encapsula las reglas de negocio de un usuario del sistema
public class User
{
    // ID del usuario (Value Object)
    public UserId Id { get; private set; }

    // Nombre de usuario para iniciar sesión (Value Object con validaciones de formato)
    public UserUsername Username { get; private set; }

    // Contraseña del usuario (Value Object que valida longitud mínima)
    public UserPassword Password { get; private set; }

    // ID del rol del usuario (Administrador, Cliente...)
    public int IdUserRole { get; private set; }

    // ID de la persona vinculada al usuario (opcional: un admin puede no tener persona asociada)
    public int? IdPerson { get; private set; }

    // Indica si el usuario está activo en el sistema
    public bool Active { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private User(UserId id, UserUsername username, UserPassword password,
        int idUserRole, int? idPerson, bool active)
    {
        Id = id;
        Username = username;
        Password = password;
        IdUserRole = idUserRole;
        IdPerson = idPerson;
        Active = active;
    }

    // Método de fábrica para crear o reconstruir un usuario desde la base de datos
    public static User Create(int id, string username, string password,
        int idUserRole, int? idPerson, bool active)
    {
        // Regla: todo usuario debe tener un rol de sistema asignado válido
        if (idUserRole <= 0)
            throw new ArgumentException("IdUserRole must be greater than 0.", nameof(idUserRole));

        // Regla: si se vincula a una persona, la referencia debe ser válida
        if (idPerson.HasValue && idPerson.Value <= 0)
            throw new ArgumentException("IdPerson must be greater than 0.", nameof(idPerson));

        // Regla: username y password son validados por sus Value Objects (formato y longitud mínima)
        return new User(
            UserId.Create(id),
            UserUsername.Create(username),
            UserPassword.Create(password),
            idUserRole,
            idPerson,
            active
        );
    }

    // Método de fábrica para crear un usuario nuevo (ID = 0, la BD lo asigna después)
    public static User CreateNew(string username, string password, int idUserRole, int? idPerson, bool active)
        => Create(0, username, password, idUserRole, idPerson, active);
}
