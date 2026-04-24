namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.valueObject;

// Value Object para el ID del método de pago
public sealed record PaymentMethodId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PaymentMethodId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static PaymentMethodId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("PaymentMethodId must be greater than 0.", nameof(value));

        return new PaymentMethodId(value);
    }

    public override string ToString() => Value.ToString();
}
