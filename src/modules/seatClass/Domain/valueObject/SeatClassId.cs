namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.valueObject;

// Value Object para el ID de la clase de asiento
public sealed record SeatClassId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private SeatClassId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static SeatClassId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("SeatClassId must be greater than 0.", nameof(value));

        return new SeatClassId(value);
    }

    public override string ToString() => Value.ToString();
}
