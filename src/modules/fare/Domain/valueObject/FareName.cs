using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;

// Value Object para el nombre de la tarifa, permite letras, números, espacios y guiones
public sealed record FareName
{
    // Se permiten números para nombres como "Business 1", "Economy 2"
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ0-9\s\-]+$", RegexOptions.Compiled);

    // El valor del nombre de la tarifa
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FareName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 100 caracteres y tenga caracteres válidos
    public static FareName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Fare name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 100)
            throw new ArgumentException("Fare name cannot exceed 100 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Fare name contains invalid characters.", nameof(value));

        return new FareName(value);
    }

    public override string ToString() => Value;
}
