namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

// Value Object para los asientos disponibles en el vuelo al momento de la consulta
public sealed record FlightAvailableSeats
{
    // Asientos que aún no han sido reservados — baja con cada tiquete emitido
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FlightAvailableSeats(int value) => Value = value;

    // Valida que no sea negativo y no supere el límite de 1000 asientos
    public static FlightAvailableSeats Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("Available seats cannot be negative.", nameof(value));

        if (value > 1000)
            throw new ArgumentException("Available seats cannot exceed 1000.", nameof(value));

        return new FlightAvailableSeats(value);
    }

    public override string ToString() => Value.ToString();
}
