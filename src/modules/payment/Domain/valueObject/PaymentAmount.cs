namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.valueObject;

// Value Object para el monto del pago — garantiza que siempre sea un valor positivo
public sealed record PaymentAmount
{
    // El monto en decimal con 2 cifras
    public decimal Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PaymentAmount(decimal value) => Value = value;

    // Coherente con tarifas/pagos en COP hasta 15 millones
    private const decimal MaxAmount = 15_000_000m;

    // Valida que el monto sea positivo y no supere el límite, y redondea a 2 decimales
    public static PaymentAmount Create(decimal value)
    {
        if (value <= 0)
            throw new ArgumentException("Payment amount must be greater than 0.", nameof(value));

        if (value > MaxAmount)
            throw new ArgumentException($"Payment amount cannot exceed {MaxAmount:N0} COP.", nameof(value));

        return new PaymentAmount(Math.Round(value, 2));
    }

    public override string ToString() => Value.ToString("F2");
}
