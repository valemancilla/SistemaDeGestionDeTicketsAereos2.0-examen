namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.valueObject;

// Value Object para la distancia en kilómetros entre origen y destino de la ruta
public sealed record RouteDistanceKm
{
    // El valor de la distancia en km — máximo 25,000 km (aprox. vuelo más largo del mundo)
    public decimal Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private RouteDistanceKm(decimal value) => Value = value;

    // Valida que la distancia sea positiva y no supere el límite máximo razonable
    public static RouteDistanceKm Create(decimal value)
    {
        if (value <= 0)
            throw new ArgumentException("Route distance must be greater than 0.", nameof(value));

        if (value > 25000)
            throw new ArgumentException("Route distance cannot exceed 25,000 km.", nameof(value));

        return new RouteDistanceKm(value);
    }

    public override string ToString() => Value.ToString("F2");
}
