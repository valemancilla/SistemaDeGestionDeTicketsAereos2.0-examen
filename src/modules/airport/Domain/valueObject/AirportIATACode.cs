using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.valueObject;

// Value Object para el código IATA del aeropuerto, debe ser exactamente 3 letras mayúsculas
public sealed record AirportIATACode
{
    // Patrón que obliga a que el código sea exactamente 3 letras mayúsculas (ej: BOG, MIA, JFK)
    private static readonly Regex ValidPattern = new(@"^[A-Z]{3}$", RegexOptions.Compiled);

    // El valor del código IATA
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private AirportIATACode(string value) => Value = value;

    // Valida que el código no esté vacío y tenga exactamente 3 letras mayúsculas
    public static AirportIATACode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Airport IATA code cannot be empty.", nameof(value));

        // Se normaliza a mayúsculas por si el usuario lo ingresó en minúsculas
        value = value.Trim().ToUpper();

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Airport IATA code must be exactly 3 uppercase letters.", nameof(value));

        return new AirportIATACode(value);
    }

    public override string ToString() => Value;
}
