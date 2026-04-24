namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.valueObject;

// Value Object para el ID del registro BookingCustomer
public sealed record BookingCustomerId
{
    // El valor numérico del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BookingCustomerId(int value) => Value = value;

    // Valida que el ID no sea negativo (0 es válido para registros nuevos)
    public static BookingCustomerId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("BookingCustomerId must be greater than 0.", nameof(value));

        return new BookingCustomerId(value);
    }

    public override string ToString() => Value.ToString();
}
