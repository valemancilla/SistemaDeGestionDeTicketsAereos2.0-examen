namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.valueObject;

// Value Object para el ID del teléfono del cliente
public sealed record CustomerPhoneId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CustomerPhoneId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static CustomerPhoneId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("CustomerPhoneId must be greater than 0.", nameof(value));

        return new CustomerPhoneId(value);
    }

    public override string ToString() => Value.ToString();
}
