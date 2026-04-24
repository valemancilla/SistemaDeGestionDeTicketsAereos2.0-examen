namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.valueObject;

// Value Object para la fecha y hora en que se realizó el pago
public sealed record PaymentDate
{
    // Incluye hora porque los pagos pueden ocurrir varias veces al día
    public DateTime Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PaymentDate(DateTime value) => Value = value;

    // Valida que la fecha no esté vacía y que no sea futura
    public static PaymentDate Create(DateTime value)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Payment date cannot be empty.", nameof(value));

        if (value > DateTime.Now)
            throw new ArgumentException("Payment date cannot be in the future.", nameof(value));

        return new PaymentDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");
}
