namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.valueObject;

// Value Object para la observación del cambio de estado de una reserva — es opcional
public sealed record BookingStatusHistoryObservation
{
    // El texto de la observación (puede ser null si no se necesita explicación)
    public string? Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BookingStatusHistoryObservation(string? value) => Value = value;

    // Valida que si existe observación, no exceda la longitud máxima permitida
    public static BookingStatusHistoryObservation Create(string? value)
    {
        // Si hay texto, se limita a 500 caracteres para no saturar el almacenamiento
        if (value != null && value.Trim().Length > 500)
            throw new ArgumentException("Observation cannot exceed 500 characters.", nameof(value));

        return new BookingStatusHistoryObservation(value?.Trim());
    }

    public override string ToString() => Value ?? string.Empty;
}
