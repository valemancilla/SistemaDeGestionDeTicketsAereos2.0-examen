using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.valueObject;

// Value Object para el nombre de usuario con el que inicia sesión en el sistema
public sealed record UserUsername
{
    // Solo letras, dígitos y guion bajo — sin espacios ni caracteres especiales para mayor seguridad
    private static readonly Regex ValidPattern = new(@"^[a-zA-Z0-9_]+$", RegexOptions.Compiled);

    // El valor del nombre de usuario
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private UserUsername(string value) => Value = value;

    // Valida formato, longitud mínima (3) y máxima (50) del nombre de usuario
    public static UserUsername Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be empty.", nameof(value));

        value = value.Trim();

        // Regla: un username muy corto no es lo suficientemente identificable
        if (value.Length < 3)
            throw new ArgumentException("Username must be at least 3 characters.", nameof(value));

        // Regla: límite razonable para no saturar la columna en la base de datos
        if (value.Length > 50)
            throw new ArgumentException("Username cannot exceed 50 characters.", nameof(value));

        // Regla: evitar caracteres que puedan causar problemas en consultas o URLs
        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Username can only contain letters, digits, and underscores.", nameof(value));

        return new UserUsername(value);
    }

    public override string ToString() => Value;
}
