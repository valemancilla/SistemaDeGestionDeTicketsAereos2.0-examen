namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.valueObject;

// Value Object para el ID del pago
public sealed record PaymentId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PaymentId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static PaymentId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("PaymentId must be greater than 0.", nameof(value));

        return new PaymentId(value);
    }

    public override string ToString() => Value.ToString();
}
