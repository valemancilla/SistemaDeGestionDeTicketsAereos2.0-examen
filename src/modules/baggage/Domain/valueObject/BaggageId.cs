namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.valueObject;

// Value Object para el ID del equipaje
public sealed record BaggageId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BaggageId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static BaggageId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("BaggageId must be greater than 0.", nameof(value));

        return new BaggageId(value);
    }

    public override string ToString() => Value.ToString();
}
