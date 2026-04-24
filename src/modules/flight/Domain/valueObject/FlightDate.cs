namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

// Value Object para la fecha de operación del vuelo
public sealed record FlightDate
{
    // La fecha en que el vuelo despega
    public DateOnly Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FlightDate(DateOnly value) => Value = value;

    // Valida que la fecha no esté vacía — se pueden programar vuelos futuros
    public static FlightDate Create(DateOnly value)
    {
        if (value == DateOnly.MinValue)
            throw new ArgumentException("Flight date cannot be empty.", nameof(value));

        return new FlightDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");
}
