namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.valueObject;

// Value Object para el ID del registro de historial de estado de reserva
public sealed record BookingStatusHistoryId
{
    // El valor numérico del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BookingStatusHistoryId(int value) => Value = value;

    // Valida que el ID no sea negativo (0 es válido para registros nuevos)
    public static BookingStatusHistoryId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("BookingStatusHistoryId must be greater than 0.", nameof(value));

        return new BookingStatusHistoryId(value);
    }

    public override string ToString() => Value.ToString();
}
