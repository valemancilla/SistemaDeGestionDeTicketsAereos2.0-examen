using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.valueObject;

// Value Object para el tipo de entidad al que aplica el estado (ej: "Flight", "Booking", "Ticket")
public sealed record SystemStatusEntityType
{
    // Solo letras, sin espacios — debe coincidir con el nombre de la entidad en el sistema
    private static readonly Regex ValidPattern = new(@"^[a-zA-Z]+$", RegexOptions.Compiled);

    // El valor del tipo de entidad
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private SystemStatusEntityType(string value) => Value = value;

    // Valida que el tipo no esté vacío, no exceda 50 caracteres y solo contenga letras
    public static SystemStatusEntityType Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Entity type cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 50)
            throw new ArgumentException("Entity type cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Entity type can only contain letters.", nameof(value));

        return new SystemStatusEntityType(value);
    }

    public override string ToString() => Value;
}
