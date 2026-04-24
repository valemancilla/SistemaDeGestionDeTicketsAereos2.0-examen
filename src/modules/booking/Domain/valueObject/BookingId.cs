// Value Object para el identificador de una reserva, no puede ser negativo
namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;

public sealed record BookingId
{
    // El valor numérico del ID
    public int Value { get; }

    // Constructor privado para forzar el uso del método Create
    private BookingId(int value) => Value = value;

    // Valida que el ID no sea negativo (0 es válido cuando la BD aún no lo asignó)
    public static BookingId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("BookingId must be greater than 0.", nameof(value));

        return new BookingId(value);
    }

    public override string ToString() => Value.ToString();
}
