namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.valueObject;

// Value Object para el ID de la asignación asiento-vuelo
public sealed record SeatFlightId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private SeatFlightId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static SeatFlightId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("SeatFlightId must be greater than 0.", nameof(value));

        return new SeatFlightId(value);
    }

    public override string ToString() => Value.ToString();
}
