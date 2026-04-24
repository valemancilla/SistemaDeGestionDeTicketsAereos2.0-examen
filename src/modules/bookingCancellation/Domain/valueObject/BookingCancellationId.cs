// Value Object para el ID de una cancelación de reserva, no puede ser negativo
namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.valueObject;

public sealed record BookingCancellationId
{
    // El valor numérico del ID
    public int Value { get; }

    // Constructor privado
    private BookingCancellationId(int value) => Value = value;

    // Valida que el ID no sea negativo (0 es permitido para registros nuevos)
    public static BookingCancellationId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("BookingCancellationId must be greater than 0.", nameof(value));

        return new BookingCancellationId(value);
    }

    public override string ToString() => Value.ToString();
}
