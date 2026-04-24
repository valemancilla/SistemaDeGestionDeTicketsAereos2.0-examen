namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;

// Value Object para el ID de la persona
public sealed record PersonId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PersonId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static PersonId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("PersonId must be greater than 0.", nameof(value));

        return new PersonId(value);
    }

    public override string ToString() => Value.ToString();
}
