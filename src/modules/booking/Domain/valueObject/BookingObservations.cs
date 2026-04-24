// Value Object para las observaciones de una reserva: es opcional, pero si existe no puede exceder 500 caracteres
namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;

public sealed record BookingObservations
{
    // El texto de las observaciones (puede ser null si el cliente no deja ninguna)
    public string? Value { get; }

    // Constructor privado
    private BookingObservations(string? value) => Value = value;

    // Si se pasan observaciones, valida que no sean demasiado largas antes de aceptarlas
    public static BookingObservations Create(string? value)
    {
        // Las observaciones son opcionales, pero si existen tienen un límite de longitud
        if (value != null && value.Trim().Length > 500)
            throw new ArgumentException("Observations cannot exceed 500 characters.", nameof(value));

        return new BookingObservations(value?.Trim());
    }

    public override string ToString() => Value ?? string.Empty;
}
