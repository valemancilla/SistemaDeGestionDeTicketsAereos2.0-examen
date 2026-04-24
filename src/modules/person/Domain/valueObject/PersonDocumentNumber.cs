using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;

// Value Object para el número de documento de identidad de la persona
public sealed record PersonDocumentNumber
{
    // Entre 4 y 20 caracteres alfanuméricos con guiones — cubre cédulas, pasaportes y tarjetas
    private static readonly Regex ValidPattern = new(@"^[a-zA-Z0-9\-]{4,20}$", RegexOptions.Compiled);

    // El valor del número de documento, normalizado a mayúsculas
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PersonDocumentNumber(string value) => Value = value;

    // Valida el formato y normaliza a mayúsculas para evitar duplicados por diferencia de casing
    public static PersonDocumentNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Document number cannot be empty.", nameof(value));

        value = value.Trim().ToUpper();

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Document number must be 4 to 20 alphanumeric characters (hyphens allowed).", nameof(value));

        return new PersonDocumentNumber(value);
    }

    public override string ToString() => Value;
}
