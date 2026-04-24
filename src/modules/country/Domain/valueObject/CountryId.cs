namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.valueObject;

// Value Object para el ID del país
public sealed record CountryId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CountryId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static CountryId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("CountryId must be greater than 0.", nameof(value));

        return new CountryId(value);
    }

    public override string ToString() => Value.ToString();
}
