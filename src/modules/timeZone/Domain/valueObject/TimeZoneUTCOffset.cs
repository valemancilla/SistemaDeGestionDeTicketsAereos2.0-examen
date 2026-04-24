using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.valueObject;

// Value Object para el desplazamiento UTC de la zona horaria (ej: "-05:00", "UTC-5" de datos semilla)
public sealed record TimeZoneUTCOffset
{
    // Formato ISO: +HH:MM o -HH:MM
    private static readonly Regex IsoOffsetPattern = new(@"^[+-]\d{2}:\d{2}$", RegexOptions.Compiled);

    // Formato legado en semillas BD: UTC-5, UTC+1, UTC+0
    private static readonly Regex LegacyUtcPattern = new(@"^UTC[+-]\d{1,2}$", RegexOptions.Compiled);

    // El valor del offset UTC
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private TimeZoneUTCOffset(string value) => Value = value;

    // Valida offset estilo ISO (+HH:MM) o legado (UTC±n) usado en migraciones.
    public static TimeZoneUTCOffset Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("UTC offset cannot be empty.", nameof(value));

        value = value.Trim();

        if (!IsoOffsetPattern.IsMatch(value) && !LegacyUtcPattern.IsMatch(value))
            throw new ArgumentException(
                "El offset debe ser +HH:MM o -HH:MM (ej. -05:00), o el formato UTC±n (ej. UTC-5).",
                nameof(value));

        return new TimeZoneUTCOffset(value);
    }

    public override string ToString() => Value;
}
