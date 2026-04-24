namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;

// Value Object para la fecha de inicio de vigencia de la tarifa
public sealed record FareValidFrom
{
    // La fecha desde la que la tarifa está disponible para venta
    public DateOnly Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FareValidFrom(DateOnly value) => Value = value;

    // Valida que la fecha no esté vacía — la coherencia con ValidTo se verifica en el agregado
    public static FareValidFrom Create(DateOnly value)
    {
        if (value == DateOnly.MinValue)
            throw new ArgumentException("Fare valid-from date cannot be empty.", nameof(value));

        return new FareValidFrom(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");
}
