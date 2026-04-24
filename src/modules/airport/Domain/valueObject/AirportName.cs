using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.valueObject;

// Value Object para el nombre del aeropuerto, permite letras con acentos, números, espacios y símbolos comunes
public sealed record AirportName
{
    // Patrón que permite letras (incluyendo acentos), números, espacios, guiones, apóstrofes y puntos
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ0-9\s\-'\.]+$", RegexOptions.Compiled);

    // El valor del nombre del aeropuerto
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private AirportName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 100 caracteres y tenga caracteres válidos
    public static AirportName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Airport name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 100)
            throw new ArgumentException("Airport name cannot exceed 100 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Airport name contains invalid characters.", nameof(value));

        return new AirportName(value);
    }

    public override string ToString() => Value;
}
