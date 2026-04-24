namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.valueObject;

// Value Object para el ID del tipo de equipaje
public sealed record BaggageTypeId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BaggageTypeId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static BaggageTypeId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("BaggageTypeId must be greater than 0.", nameof(value));

        return new BaggageTypeId(value);
    }

    public override string ToString() => Value.ToString();
}
