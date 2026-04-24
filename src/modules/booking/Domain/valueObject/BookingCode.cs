// Value Object para el código de reserva: debe ser alfanumérico en mayúsculas y tener entre 6 y 20 caracteres
using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;

public sealed record BookingCode
{
    // Solo se aceptan letras mayúsculas y números, entre 6 y 20 caracteres
    private static readonly Regex ValidPattern = new(@"^[A-Z0-9]{6,20}$", RegexOptions.Compiled);

    // El valor del código de reserva
    public string Value { get; }

    // Constructor privado
    private BookingCode(string value) => Value = value;

    // Valida que el código no esté vacío y que tenga el formato correcto antes de crearlo
    public static BookingCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Booking code cannot be empty.", nameof(value));

        // Normalizamos a mayúsculas para evitar inconsistencias
        value = value.Trim().ToUpper();

        // El código debe cumplir el patrón alfanumérico de 6 a 20 caracteres
        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Booking code must be 6 to 20 uppercase alphanumeric characters.", nameof(value));

        return new BookingCode(value);
    }

    public override string ToString() => Value;
}
