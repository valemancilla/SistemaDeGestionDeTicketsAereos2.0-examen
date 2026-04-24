namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;

// Value Object para el ID de la tarifa
public sealed record FareId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FareId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static FareId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("FareId must be greater than 0.", nameof(value));

        return new FareId(value);
    }

    public override string ToString() => Value.ToString();
}
