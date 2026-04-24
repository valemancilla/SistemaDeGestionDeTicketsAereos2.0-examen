namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.valueObject;

// Value Object para el ID de la aerolínea, evita pasar enteros sueltos sin contexto
public sealed record AirlineId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private AirlineId(int value) => Value = value;

    // Valida que el ID no sea negativo (0 es válido para aerolíneas aún no guardadas)
    public static AirlineId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("AirlineId must be greater than 0.", nameof(value));

        return new AirlineId(value);
    }

    public override string ToString() => Value.ToString();
}
