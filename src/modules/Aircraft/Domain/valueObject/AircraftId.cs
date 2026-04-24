namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.valueObject;

// Value Object para el ID del avión, evita pasar enteros sueltos y sin significado
public sealed record AircraftId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private AircraftId(int value) => Value = value;

    // Valida que el ID no sea negativo (0 es válido para aviones aún no guardados)
    public static AircraftId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("AircraftId must be greater than 0.", nameof(value));

        return new AircraftId(value);
    }

    public override string ToString() => Value.ToString();
}
