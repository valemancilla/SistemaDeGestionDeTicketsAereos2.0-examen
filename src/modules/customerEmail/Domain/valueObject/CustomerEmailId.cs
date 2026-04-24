namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.valueObject;

// Value Object para el ID del correo electrónico del cliente
public sealed record CustomerEmailId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CustomerEmailId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static CustomerEmailId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("CustomerEmailId must be greater than 0.", nameof(value));

        return new CustomerEmailId(value);
    }

    public override string ToString() => Value.ToString();
}
