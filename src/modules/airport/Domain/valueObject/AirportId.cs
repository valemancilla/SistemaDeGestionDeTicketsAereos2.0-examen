namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.valueObject;

// Value Object para el ID del aeropuerto
public sealed record AirportId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private AirportId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static AirportId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("AirportId must be greater than 0.", nameof(value));

        return new AirportId(value);
    }

    public override string ToString() => Value.ToString();
}
