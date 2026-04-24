using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.valueObject;

// Value Object para el nombre de la clase de asiento
public sealed record SeatClassName
{
    // Ej: "Económica", "Business", "Primera Clase"
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-]+$", RegexOptions.Compiled);

    // El valor del nombre de la clase
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private SeatClassName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 50 caracteres y tenga caracteres válidos
    public static SeatClassName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Seat class name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 50)
            throw new ArgumentException("Seat class name cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Seat class name contains invalid characters.", nameof(value));

        return new SeatClassName(value);
    }

    public override string ToString() => Value;
}
