using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.valueObject;

// Value Object para el nombre del fabricante de aeronaves
public sealed record ManufacturerName
{
    // Se permiten números, puntos y apóstrofos para nombres como "McDonnell Douglas" o "de Havilland"
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ0-9\s\-'\.]+$", RegexOptions.Compiled);

    // El valor del nombre del fabricante
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private ManufacturerName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 100 caracteres y tenga caracteres válidos
    public static ManufacturerName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Manufacturer name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 100)
            throw new ArgumentException("Manufacturer name cannot exceed 100 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Manufacturer name contains invalid characters.", nameof(value));

        return new ManufacturerName(value);
    }

    public override string ToString() => Value;
}
