using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.valueObject;

// Value Object para el nombre del rol de usuario, solo letras con acentos, espacios y guiones
public sealed record RoleName
{
    // Ej: "Administrador", "Cliente", "Agente de Mostrador"
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-]+$", RegexOptions.Compiled);

    // El valor del nombre del rol
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private RoleName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 50 caracteres y tenga caracteres válidos
    public static RoleName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Role name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 50)
            throw new ArgumentException("Role name cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Role name contains invalid characters.", nameof(value));

        return new RoleName(value);
    }

    public override string ToString() => Value;
}
