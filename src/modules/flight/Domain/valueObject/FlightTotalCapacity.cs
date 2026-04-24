namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

// Value Object para la capacidad total de asientos del vuelo
public sealed record FlightTotalCapacity
{
    // La cantidad máxima de pasajeros que puede transportar la aeronave
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FlightTotalCapacity(int value) => Value = value;

    // Valida que la capacidad sea positiva y no supere el límite razonable de 1000
    public static FlightTotalCapacity Create(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Flight total capacity must be greater than 0.", nameof(value));

        if (value > 1000)
            throw new ArgumentException("Flight total capacity cannot exceed 1000.", nameof(value));

        return new FlightTotalCapacity(value);
    }

    public override string ToString() => Value.ToString();
}
