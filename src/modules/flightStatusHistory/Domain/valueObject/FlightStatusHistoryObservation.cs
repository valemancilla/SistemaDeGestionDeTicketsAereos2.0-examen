namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.valueObject;

// Value Object para la observación opcional del cambio de estado del vuelo
public sealed record FlightStatusHistoryObservation
{
    // Puede ser null si el cambio de estado no requiere explicación adicional
    public string? Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FlightStatusHistoryObservation(string? value) => Value = value;

    // Valida que la observación no supere 500 caracteres si está presente
    public static FlightStatusHistoryObservation Create(string? value)
    {
        if (value != null && value.Trim().Length > 500)
            throw new ArgumentException("Observation cannot exceed 500 characters.", nameof(value));

        return new FlightStatusHistoryObservation(value?.Trim());
    }

    public override string ToString() => Value ?? string.Empty;
}
