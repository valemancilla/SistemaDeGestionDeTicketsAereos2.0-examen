namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.valueObject;

// Value Object para el peso del equipaje, garantiza que sea mayor a 0 y no exceda el máximo por pieza
public sealed record BaggageWeight
{
    /// <summary>Peso máximo permitido por pieza de equipaje (kg), alineado con reglas típicas de aerolínea.</summary>
    public const decimal MaximumKilograms = 50m;

    // El peso en kilogramos (con 2 decimales)
    public decimal Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BaggageWeight(decimal value) => Value = value;

    // Valida que el peso sea mayor que 0 y no supere el máximo; lo redondea a 2 decimales
    public static BaggageWeight Create(decimal value)
    {
        if (value <= 0)
            throw new ArgumentException(
                "El peso debe ser mayor que 0 kg. Indica el peso aproximado de la maleta (por ejemplo, equipaje de cabina suele ir entre 7 y 12 kg, y de bodega entre 15 y 32 kg).");

        if (value > MaximumKilograms)
            throw new ArgumentException(
                $"El peso por pieza no puede superar {MaximumKilograms:0} kg. Ingresa un valor mayor que 0 y hasta {MaximumKilograms:0} kg (referencia: cabina ~7–12 kg, bodega ~15–32 kg, piezas muy pesadas rara vez pasan de 40 kg).");

        // Se redondea para evitar problemas de precisión con decimales
        return new BaggageWeight(Math.Round(value, 2));
    }

    public override string ToString() => $"{Value:F2} kg";
}
