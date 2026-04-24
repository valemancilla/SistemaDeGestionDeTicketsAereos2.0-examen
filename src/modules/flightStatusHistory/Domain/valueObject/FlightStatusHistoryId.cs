namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.valueObject;

// Value Object para el ID del registro de historial de estado de vuelo
public sealed record FlightStatusHistoryId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FlightStatusHistoryId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static FlightStatusHistoryId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("FlightStatusHistoryId must be greater than 0.", nameof(value));

        return new FlightStatusHistoryId(value);
    }

    public override string ToString() => Value.ToString();
}
