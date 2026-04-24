namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.valueObject;

// Value Object para la capacidad del avión, garantiza que sea un número válido entre 1 y 1000
public sealed record AircraftCapacity
{
    // El valor entero de la capacidad (cantidad de asientos)
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private AircraftCapacity(int value) => Value = value;

    // Valida que la capacidad sea mayor a 0 y no exceda los 1000 asientos
    public static AircraftCapacity Create(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Aircraft capacity must be greater than 0.", nameof(value));

        if (value > 1000)
            throw new ArgumentException("Aircraft capacity cannot exceed 1000 seats.", nameof(value));

        return new AircraftCapacity(value);
    }

    public override string ToString() => Value.ToString();
}
