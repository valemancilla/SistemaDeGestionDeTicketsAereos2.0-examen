namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.valueObject;

// Value Object para la observación opcional del cambio de estado del tiquete
public sealed record TicketStatusHistoryObservation
{
    // Puede ser null si el cambio es automático y no requiere explicación
    public string? Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private TicketStatusHistoryObservation(string? value) => Value = value;

    // Valida que la observación no supere 500 caracteres si está presente
    public static TicketStatusHistoryObservation Create(string? value)
    {
        if (value != null && value.Trim().Length > 500)
            throw new ArgumentException("Observation cannot exceed 500 characters.", nameof(value));

        return new TicketStatusHistoryObservation(value?.Trim());
    }

    public override string ToString() => Value ?? string.Empty;
}
