namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.valueObject;

// Value Object para la contraseña del usuario — solo valida longitud, no el hash
// El hashing real se hace en la capa de aplicación antes de llamar a Create
public sealed record UserPassword
{
    // El valor de la contraseña (ya hasheada cuando proviene de la BD)
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private UserPassword(string value) => Value = value;

    // Valida que la contraseña tenga una longitud aceptable
    public static UserPassword Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Password cannot be empty.", nameof(value));

        // Regla: mínimo 8 caracteres para dificultar ataques de fuerza bruta
        if (value.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters.", nameof(value));

        // Regla: límite alto para acomodar hashes bcrypt/argon2 sin truncar
        if (value.Length > 255)
            throw new ArgumentException("Password cannot exceed 255 characters.", nameof(value));

        return new UserPassword(value);
    }

    // Oculta la contraseña al hacer ToString() para evitar que se filtre en logs o consola
    public override string ToString() => "********";
}
