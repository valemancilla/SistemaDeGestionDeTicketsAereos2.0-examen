using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.valueObject;

// Value Object para la dirección de correo electrónico del cliente
public sealed record CustomerEmailAddress
{
    // Patrón básico de email: algo@algo.algo — sin espacios ni dobles @
    private static readonly Regex ValidPattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // El valor del correo, normalizado a minúsculas
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CustomerEmailAddress(string value) => Value = value;

    // Valida el formato del email y normaliza a minúsculas para evitar duplicados por mayúsculas
    public static CustomerEmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email address cannot be empty.", nameof(value));

        value = value.Trim().ToLower();

        if (value.Length > 255)
            throw new ArgumentException("Email address cannot exceed 255 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Email address is not valid. It must contain '@' and a valid domain.", nameof(value));

        return new CustomerEmailAddress(value);
    }

    public override string ToString() => Value;
}
