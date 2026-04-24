using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.valueObject;

// Value Object para el nombre del grupo de tripulación: letras (cualquier idioma), números, espacio y signos comunes.
public sealed record CrewGroupName
{
    // Incluye paréntesis, punto, coma y guión (p. ej. semillas o nombres descriptivos ya guardados en BD).
    private static readonly Regex ValidPattern = new(@"^[\p{L}\p{N}\s().,_\-]+$", RegexOptions.Compiled);

    // El valor del nombre del grupo
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CrewGroupName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 100 caracteres y tenga caracteres válidos
    public static CrewGroupName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre del grupo de tripulación no puede estar vacío.", nameof(value));

        value = value.Trim();

        if (value.Length > 100)
            throw new ArgumentException("El nombre del grupo no puede superar 100 caracteres.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException(
                "El nombre del grupo contiene caracteres no permitidos (use letras, números, espacio, guión, punto, coma, paréntesis o guion bajo).",
                nameof(value));

        return new CrewGroupName(value);
    }

    public override string ToString() => Value;
}
