using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.valueObject;

// Value Object para el código IATA de la aerolínea, debe ser exactamente 2 letras mayúsculas
public sealed record AirlineIATACode
{
    // Patrón que obliga a que el código sea exactamente 2 letras mayúsculas (ej: AV, LA, AA)
    private static readonly Regex ValidPattern = new(@"^[A-Z]{2}$", RegexOptions.Compiled);

    // El valor del código IATA
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private AirlineIATACode(string value) => Value = value;

    // Valida que el código no esté vacío y tenga exactamente 2 letras mayúsculas
    public static AirlineIATACode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Airline IATA code cannot be empty.", nameof(value));

        // Se normaliza a mayúsculas por si el usuario lo ingresó en minúsculas
        value = value.Trim().ToUpper();

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Airline IATA code must be exactly 2 uppercase letters.", nameof(value));

        return new AirlineIATACode(value);
    }

    public override string ToString() => Value;
}
