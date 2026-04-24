namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.valueObject;

// Value Object para el ID del género
public sealed record GenderId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private GenderId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static GenderId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("GenderId must be greater than 0.", nameof(value));

        return new GenderId(value);
    }

    public override string ToString() => Value.ToString();
}
