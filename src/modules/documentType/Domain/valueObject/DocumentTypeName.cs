using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.valueObject;

// Value Object para el nombre del tipo de documento, solo letras, espacios y guiones
public sealed record DocumentTypeName
{
    // Ej: "Cédula de Ciudadanía", "Pasaporte", "Tarjeta de Identidad"
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-]+$", RegexOptions.Compiled);

    // El valor del nombre del tipo de documento
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private DocumentTypeName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 50 caracteres y tenga caracteres válidos
    public static DocumentTypeName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Document type name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 50)
            throw new ArgumentException("Document type name cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Document type name contains invalid characters.", nameof(value));

        return new DocumentTypeName(value);
    }

    public override string ToString() => Value;
}
