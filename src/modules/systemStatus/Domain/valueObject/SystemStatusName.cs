using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.valueObject;

// Value Object para el nombre del estado del sistema (ej: "Activo", "Cancelado", "Pendiente de Pago")
public sealed record SystemStatusName
{
    // Letras con acentos, espacios y guiones — cubre nombres descriptivos en español
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-]+$", RegexOptions.Compiled);

    // El valor del nombre del estado
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private SystemStatusName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 100 caracteres y tenga caracteres válidos
    public static SystemStatusName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("System status name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 100)
            throw new ArgumentException("System status name cannot exceed 100 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("System status name contains invalid characters.", nameof(value));

        return new SystemStatusName(value);
    }

    public override string ToString() => Value;
}
