namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.valueObject;

// Value Object para el ID de la ciudad
public sealed record CityId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CityId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static CityId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("CityId must be greater than 0.", nameof(value));

        return new CityId(value);
    }

    public override string ToString() => Value.ToString();
}
