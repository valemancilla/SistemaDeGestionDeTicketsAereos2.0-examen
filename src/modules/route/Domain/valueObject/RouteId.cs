namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.valueObject;

// Value Object para el ID de la ruta aérea
public sealed record RouteId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private RouteId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static RouteId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("RouteId must be greater than 0.", nameof(value));

        return new RouteId(value);
    }

    public override string ToString() => Value.ToString();
}
