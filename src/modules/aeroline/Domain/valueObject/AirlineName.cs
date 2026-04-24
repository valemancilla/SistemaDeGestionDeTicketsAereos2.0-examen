using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.valueObject;

// Value Object para el nombre de la aerolínea, garantiza que sea un nombre válido y no vacío
public sealed record AirlineName
{
    // Patrón que permite letras (con acentos), números, espacios, guiones, apóstrofes y puntos
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ0-9\s\-'\.]+$", RegexOptions.Compiled);

    // El valor del nombre de la aerolínea
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private AirlineName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 100 caracteres y tenga caracteres válidos
    public static AirlineName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Airline name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 100)
            throw new ArgumentException("Airline name cannot exceed 100 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Airline name contains invalid characters.", nameof(value));

        return new AirlineName(value);
    }

    public override string ToString() => Value;
}
