namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

// Value Object para el ID del vuelo
public sealed record FlightId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FlightId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static FlightId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("FlightId must be greater than 0.", nameof(value));

        return new FlightId(value);
    }

    public override string ToString() => Value.ToString();
}
