using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;

// Value Object para el nombre de la persona, permite letras con acentos, espacios, guiones y apóstrofos
public sealed record PersonFirstName
{
    // Cubre nombres como "María José", "Jean-Pierre", "D'Angelo"
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-']+$", RegexOptions.Compiled);

    // El valor del nombre
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PersonFirstName(string value) => Value = value;

    // Valida longitud mínima de 2 caracteres, máxima de 50, y caracteres permitidos
    public static PersonFirstName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("First name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length < 2)
            throw new ArgumentException("First name must be at least 2 characters.", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("First name cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("First name can only contain letters, spaces, hyphens, and apostrophes.", nameof(value));

        return new PersonFirstName(value);
    }

    public override string ToString() => Value;
}
