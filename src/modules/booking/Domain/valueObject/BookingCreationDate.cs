// Value Object para la fecha de creación de una reserva: no puede ser vacía ni futura
namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;

public sealed record BookingCreationDate
{
    // Fecha en que se creó la reserva
    public DateOnly Value { get; }

    // Constructor privado
    private BookingCreationDate(DateOnly value) => Value = value;

    // Valida que la fecha sea real y que no sea un día futuro (no se puede registrar algo que aún no pasó)
    public static BookingCreationDate Create(DateOnly value)
    {
        if (value == DateOnly.MinValue)
            throw new ArgumentException("Booking creation date cannot be empty.", nameof(value));

        // Una reserva no puede registrarse con fecha futura
        if (value > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Booking creation date cannot be in the future.", nameof(value));

        return new BookingCreationDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");
}
