namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.valueObject;

// Value Object para el ID del asiento
public sealed record SeatId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private SeatId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static SeatId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("SeatId must be greater than 0.", nameof(value));

        return new SeatId(value);
    }

    public override string ToString() => Value.ToString();
}
