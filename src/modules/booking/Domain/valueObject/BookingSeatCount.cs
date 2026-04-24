// Value Object para la cantidad de asientos en una reserva: mínimo 1 y máximo 9 por reserva
namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;

public sealed record BookingSeatCount
{
    // Cantidad de asientos reservados
    public int Value { get; }

    // Constructor privado
    private BookingSeatCount(int value) => Value = value;

    // Valida que la cantidad sea al menos 1 y que no supere el máximo permitido por reserva
    public static BookingSeatCount Create(int value)
    {
        // Regla de negocio: no tiene sentido reservar 0 asientos
        if (value <= 0)
            throw new ArgumentException("Seat count must be greater than 0.", nameof(value));

        // Regla de negocio: las aerolíneas limitan las reservas grupales a 9 asientos por transacción
        if (value > 9)
            throw new ArgumentException("Seat count cannot exceed 9 per booking.", nameof(value));

        return new BookingSeatCount(value);
    }

    public override string ToString() => Value.ToString();
}
