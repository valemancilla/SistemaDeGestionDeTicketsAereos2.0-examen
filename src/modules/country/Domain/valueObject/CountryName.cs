using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.valueObject;

// Value Object para el nombre del país, permite letras con acentos, espacios, guiones y apóstrofos
public sealed record CountryName
{
    // Patrón que cubre nombres como "Colombia", "Costa Rica", "Côte d'Ivoire"
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-']+$", RegexOptions.Compiled);

    // El valor del nombre del país
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CountryName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 100 caracteres y tenga caracteres válidos
    public static CountryName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Country name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 100)
            throw new ArgumentException("Country name cannot exceed 100 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Country name contains invalid characters.", nameof(value));

        return new CountryName(value);
    }

    public override string ToString() => Value;
}
