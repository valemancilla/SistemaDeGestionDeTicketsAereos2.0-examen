using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.valueObject;

// Value Object para la descripción del género (ej: "Masculino", "Femenino", "No binario")
public sealed record GenderDescription
{
    // Solo letras con acentos, espacios y guiones — sin números ni símbolos
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-]+$", RegexOptions.Compiled);

    // El valor de la descripción del género
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private GenderDescription(string value) => Value = value;

    // Valida que la descripción no esté vacía, no exceda 50 caracteres y tenga caracteres válidos
    public static GenderDescription Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Gender description cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 50)
            throw new ArgumentException("Gender description cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Gender description contains invalid characters.", nameof(value));

        return new GenderDescription(value);
    }

    public override string ToString() => Value;
}
