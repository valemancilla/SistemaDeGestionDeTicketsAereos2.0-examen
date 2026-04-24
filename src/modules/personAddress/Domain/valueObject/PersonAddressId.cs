namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;

// Value Object para el ID de la dirección de persona
public sealed record PersonAddressId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PersonAddressId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static PersonAddressId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("PersonAddressId must be greater than 0.", nameof(value));

        return new PersonAddressId(value);
    }

    public override string ToString() => Value.ToString();
}
