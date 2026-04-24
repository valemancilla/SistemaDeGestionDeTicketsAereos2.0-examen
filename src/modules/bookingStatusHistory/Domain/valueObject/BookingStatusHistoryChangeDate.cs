namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.valueObject;

// Value Object para la fecha en que cambió el estado de una reserva
public sealed record BookingStatusHistoryChangeDate
{
    // Fecha y hora exacta del cambio de estado
    public DateTime Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BookingStatusHistoryChangeDate(DateTime value) => Value = value;

    // Valida que la fecha sea real y no esté en el futuro
    public static BookingStatusHistoryChangeDate Create(DateTime value)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Change date cannot be empty.", nameof(value));

        // No tiene sentido registrar un cambio de estado que aún no ha ocurrido
        if (value > DateTime.Now)
            throw new ArgumentException("Change date cannot be in the future.", nameof(value));

        return new BookingStatusHistoryChangeDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");
}
