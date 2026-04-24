namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.valueObject;

// Value Object para el ID del modelo de aeronave
public sealed record AircraftModelId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private AircraftModelId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static AircraftModelId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("AircraftModelId must be greater than 0.", nameof(value));

        return new AircraftModelId(value);
    }

    public override string ToString() => Value.ToString();
}
