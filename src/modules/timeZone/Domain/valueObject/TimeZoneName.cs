using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.valueObject;

// Value Object para el nombre de la zona horaria (ej: "America/Bogota", "Europe/Madrid")
public sealed record TimeZoneName
{
    // Letras (incl. acentos latinos), números, espacios y símbolos habituales en etiquetas o IANA (ej. America/Bogota, "Bogotá / Lima").
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ0-9\s/_.+-]+$", RegexOptions.Compiled);

    // El valor del nombre de la zona horaria
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private TimeZoneName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 100 caracteres y tenga caracteres válidos
    public static TimeZoneName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Time zone name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 100)
            throw new ArgumentException("Time zone name cannot exceed 100 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Time zone name contains invalid characters.", nameof(value));

        return new TimeZoneName(value);
    }

    public override string ToString() => Value;
}
