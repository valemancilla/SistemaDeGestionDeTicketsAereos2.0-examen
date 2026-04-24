// Value Object para la fecha del vuelo en una reserva, no puede ser un valor vacío (MinValue)
namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;

public sealed record BookingFlightDate
{
    // Fecha y hora del vuelo reservado
    public DateTime Value { get; }

    // Constructor privado
    private BookingFlightDate(DateTime value) => Value = value;

    // Valida que se haya proporcionado una fecha real (no el valor mínimo por defecto)
    public static BookingFlightDate Create(DateTime value)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Booking flight date cannot be empty.", nameof(value));

        return new BookingFlightDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");
}
