using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.valueObject;

// Value Object para el número de asiento en formato IATA (ej: "12A", "1C", "34B")
public sealed record SeatNumber
{
    // Formato: 1 a 3 dígitos seguidos de una letra mayúscula — estándar en aviación comercial
    private static readonly Regex ValidPattern = new(@"^\d{1,3}[A-Z]$", RegexOptions.Compiled);

    // El valor del número de asiento, normalizado a mayúsculas
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private SeatNumber(string value) => Value = value;

    // Valida el formato y normaliza a mayúsculas para consistencia
    public static SeatNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Seat number cannot be empty.", nameof(value));

        value = value.Trim().ToUpper();

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Seat number must follow the format: 1-3 digits followed by 1 uppercase letter (e.g., 12A, 1C).", nameof(value));

        return new SeatNumber(value);
    }

    public override string ToString() => Value;
}
