using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;

// Value Object para el apellido de la persona, mismas reglas que el nombre
public sealed record PersonLastName
{
    // Cubre apellidos como "García-López", "O'Brien", "Van der Berg"
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-']+$", RegexOptions.Compiled);

    // El valor del apellido
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PersonLastName(string value) => Value = value;

    // Valida longitud mínima de 2 caracteres, máxima de 50, y caracteres permitidos
    public static PersonLastName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Last name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length < 2)
            throw new ArgumentException("Last name must be at least 2 characters.", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Last name cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Last name can only contain letters, spaces, hyphens, and apostrophes.", nameof(value));

        return new PersonLastName(value);
    }

    public override string ToString() => Value;
}
