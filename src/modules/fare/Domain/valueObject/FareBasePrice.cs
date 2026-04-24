namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;

// Value Object para el precio base de la tarifa
public sealed record FareBasePrice
{
    // El precio en formato decimal con 2 cifras
    public decimal Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FareBasePrice(decimal value) => Value = value;

    // Límite superior en COP para tarifas realistas (hasta 15 millones)
    private const decimal MaxBasePrice = 15_000_000m;

    // Valida que el precio sea positivo y no exceda el tope, y redondea a 2 decimales
    public static FareBasePrice Create(decimal value)
    {
        if (value <= 0)
            throw new ArgumentException("Fare base price must be greater than 0.", nameof(value));

        if (value > MaxBasePrice)
            throw new ArgumentException($"Fare base price cannot exceed {MaxBasePrice:N0} COP.", nameof(value));

        return new FareBasePrice(Math.Round(value, 2));
    }

    public override string ToString() => Value.ToString("F2");
}
