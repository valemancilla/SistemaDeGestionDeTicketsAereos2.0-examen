using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.valueObject;

// Value Object para el nombre de la ciudad, permite letras con acentos, espacios, guiones y apóstrofes
public sealed record CityName
{
    // Patrón que acepta nombres como "Bogotá", "São Paulo", "Cap-d'Ail"
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-']+$", RegexOptions.Compiled);

    // El valor del nombre de la ciudad
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CityName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 100 caracteres y tenga caracteres válidos
    public static CityName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("City name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 100)
            throw new ArgumentException("City name cannot exceed 100 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("City name contains invalid characters.", nameof(value));

        return new CityName(value);
    }

    public override string ToString() => Value;
}
