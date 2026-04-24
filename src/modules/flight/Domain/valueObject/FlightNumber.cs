using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

// Value Object para el número de vuelo en formato IATA (ej: "AV123", "LA4567")
public sealed record FlightNumber
{
    // Exactamente 2 letras mayúsculas (código aerolínea) seguidas de 1 a 4 dígitos
    private static readonly Regex ValidPattern = new(@"^[A-Z]{2}\d{1,4}$", RegexOptions.Compiled);

    // El valor del número de vuelo, normalizado a mayúsculas
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FlightNumber(string value) => Value = value;

    // Valida el formato IATA y normaliza automáticamente a mayúsculas
    public static FlightNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Flight number cannot be empty.", nameof(value));

        value = value.Trim().ToUpper();

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Flight number must follow the format: 2 uppercase letters followed by 1 to 4 digits (e.g., AA1234).", nameof(value));

        return new FlightNumber(value);
    }

    public override string ToString() => Value;
}
