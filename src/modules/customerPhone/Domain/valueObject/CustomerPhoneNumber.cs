using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.valueObject;

// Value Object para el número de teléfono del cliente, soporta formato internacional
public sealed record CustomerPhoneNumber
{
    // Acepta números con prefijo +, espacios, guiones y paréntesis — ej: "+57 300 123-4567"
    private static readonly Regex ValidPattern = new(@"^[\+\d][\d\s\-\(\)]{6,19}$", RegexOptions.Compiled);

    // El valor del número de teléfono
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CustomerPhoneNumber(string value) => Value = value;

    // Valida que el número tenga formato válido (7-20 caracteres incluyendo separadores)
    public static CustomerPhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty.", nameof(value));

        value = value.Trim();

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Phone number is not valid. It must be 7 to 20 characters and can contain digits, spaces, hyphens, parentheses, and a leading '+'.", nameof(value));

        return new CustomerPhoneNumber(value);
    }

    public override string ToString() => Value;
}
