using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;

// Value Object para interior, apartamento, unidad o referencia de casa (ej: "402", "21-256", "Casa 3")
public sealed record PersonAddressNumber
{
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ0-9\s\-\.\#\/]+$", RegexOptions.Compiled);

    // El valor del número, normalizado a mayúsculas
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PersonAddressNumber(string value) => Value = value;

    // Valida que el número no esté vacío, no exceda 10 caracteres y tenga caracteres válidos
    public static PersonAddressNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Address number cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 50)
            throw new ArgumentException("Address interior reference cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Address interior reference contains invalid characters.", nameof(value));

        return new PersonAddressNumber(value);
    }

    public override string ToString() => Value;
}
