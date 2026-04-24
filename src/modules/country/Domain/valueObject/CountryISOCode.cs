using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.valueObject;

// Value Object para el código ISO 3166-1 alpha-2 del país (ej: CO, US, MX)
public sealed record CountryISOCode
{
    // Exactamente 2 letras mayúsculas, sin números ni caracteres especiales
    private static readonly Regex ValidPattern = new(@"^[A-Z]{2}$", RegexOptions.Compiled);

    // El valor del código ISO
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CountryISOCode(string value) => Value = value;

    // Valida el formato ISO y normaliza a mayúsculas automáticamente
    public static CountryISOCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Country ISO code cannot be empty.", nameof(value));

        // Normalizamos a mayúsculas para que "co" sea válido igual que "CO"
        value = value.Trim().ToUpper();

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Country ISO code must be exactly 2 uppercase letters (ISO 3166-1 alpha-2).", nameof(value));

        return new CountryISOCode(value);
    }

    public override string ToString() => Value;
}
