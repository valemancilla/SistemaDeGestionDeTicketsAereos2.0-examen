// Value Object para el motivo de cancelación: es obligatorio, debe tener al menos 5 caracteres y no más de 500
namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.valueObject;

public sealed record BookingCancellationReason
{
    // El texto del motivo de cancelación
    public string Value { get; }

    // Constructor privado
    private BookingCancellationReason(string value) => Value = value;

    // Valida que el motivo exista, tenga longitud mínima significativa y no sea demasiado largo
    public static BookingCancellationReason Create(string value)
    {
        // Siempre debe existir un motivo para cancelar una reserva
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Cancellation reason cannot be empty.", nameof(value));

        value = value.Trim();

        // Un motivo de menos de 5 caracteres no es suficientemente descriptivo
        if (value.Length < 5)
            throw new ArgumentException("Cancellation reason must be at least 5 characters.", nameof(value));

        // Límite máximo para no sobrecargar el almacenamiento
        if (value.Length > 500)
            throw new ArgumentException("Cancellation reason cannot exceed 500 characters.", nameof(value));

        return new BookingCancellationReason(value);
    }

    public override string ToString() => Value;
}
