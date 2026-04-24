using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.valueObject;

// Value Object para el nombre del modelo de aeronave, permite letras, números, espacios, guiones y barras
public sealed record AircraftModelName
{
    // Patrón válido: letras, números, espacios, guiones y barras (ej: "Boeing 737-800", "A/320")
    private static readonly Regex ValidPattern = new(@"^[a-zA-Z0-9\s\-\/]+$", RegexOptions.Compiled);

    // El valor del nombre del modelo
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private AircraftModelName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 50 caracteres y tenga caracteres válidos
    public static AircraftModelName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Aircraft model name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 50)
            throw new ArgumentException("Aircraft model name cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Aircraft model name contains invalid characters.", nameof(value));

        return new AircraftModelName(value);
    }

    public override string ToString() => Value;
}
