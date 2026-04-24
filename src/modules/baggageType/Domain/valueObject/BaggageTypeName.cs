using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.valueObject;

// Value Object para el nombre del tipo de equipaje: texto legible (incluye dígitos y signos habituales en catálogos/semillas).
public sealed record BaggageTypeName
{
    // Letras (incl. latinas extendidas), números, espacios y puntuación típica (p. ej. semillas «Tarifa Basic (elegida al comprar)»).
    private static readonly Regex ValidPattern = new(@"^[\p{L}\p{N}\s\.,()\/:\-']+$", RegexOptions.Compiled);

    // El valor del nombre del tipo de equipaje
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BaggageTypeName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 50 caracteres y tenga caracteres válidos
    public static BaggageTypeName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Baggage type name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 50)
            throw new ArgumentException("Baggage type name cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Baggage type name contains invalid characters.", nameof(value));

        return new BaggageTypeName(value);
    }

    public override string ToString() => Value;
}
