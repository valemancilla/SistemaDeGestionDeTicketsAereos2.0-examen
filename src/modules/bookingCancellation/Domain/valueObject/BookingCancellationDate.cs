// Value Object para la fecha de cancelación: no puede estar vacía ni ser futura
namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.valueObject;

public sealed record BookingCancellationDate
{
    // Fecha y hora exacta en que se procesó la cancelación
    public DateTime Value { get; }

    // Constructor privado
    private BookingCancellationDate(DateTime value) => Value = value;

    // Valida que la fecha sea real y que no esté en el futuro
    public static BookingCancellationDate Create(DateTime value)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Cancellation date cannot be empty.", nameof(value));

        // No tiene sentido registrar una cancelación con fecha futura
        if (value > DateTime.Now)
            throw new ArgumentException("Cancellation date cannot be in the future.", nameof(value));

        return new BookingCancellationDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");
}
