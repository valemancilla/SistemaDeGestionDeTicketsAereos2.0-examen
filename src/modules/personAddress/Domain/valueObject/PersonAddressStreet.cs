using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;

// Value Object para la dirección completa de vía y placa (ej. "Calle 31 # 21-56", "Kra 15 # 93-47")
public sealed record PersonAddressStreet
{
    // Permite letras, números, espacios, guiones, puntos, comillas, # y comas — típico de direcciones
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ0-9\s\-\.'#,]+$", RegexOptions.Compiled);

    // El valor de la calle
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PersonAddressStreet(string value) => Value = value;

    // Valida que la calle no esté vacía, no exceda 200 caracteres y tenga caracteres válidos
    public static PersonAddressStreet Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Street cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 250)
            throw new ArgumentException("Street cannot exceed 250 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Street contains invalid characters.", nameof(value));

        return new PersonAddressStreet(value);
    }

    public override string ToString() => Value;
}
