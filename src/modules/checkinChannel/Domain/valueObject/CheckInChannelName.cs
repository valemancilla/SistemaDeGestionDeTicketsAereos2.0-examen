using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.valueObject;

// Value Object para el nombre del canal de check-in, solo permite letras, espacios y guiones
public sealed record CheckInChannelName
{
    // Patrón que permite letras con acentos, espacios y guiones (ej: "Web", "App Móvil")
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-]+$", RegexOptions.Compiled);

    // El valor del nombre del canal
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CheckInChannelName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 50 caracteres y tenga caracteres válidos
    public static CheckInChannelName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Check-in channel name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 50)
            throw new ArgumentException("Check-in channel name cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Check-in channel name contains invalid characters.", nameof(value));

        return new CheckInChannelName(value);
    }

    public override string ToString() => Value;
}
