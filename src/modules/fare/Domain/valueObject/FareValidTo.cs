namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;

// Value Object para la fecha de fin de vigencia de la tarifa
public sealed record FareValidTo
{
    // La fecha hasta la que la tarifa puede aplicarse a reservas
    public DateOnly Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FareValidTo(DateOnly value) => Value = value;

    // Valida que la fecha no esté vacía — la coherencia con ValidFrom se verifica en el agregado
    public static FareValidTo Create(DateOnly value)
    {
        if (value == DateOnly.MinValue)
            throw new ArgumentException("Fare valid-to date cannot be empty.", nameof(value));

        return new FareValidTo(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");
}
