using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;

// Barrio, sector, conjunto u otra referencia de ubicación dentro de la ciudad
public sealed record PersonAddressNeighborhood
{
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ0-9\s\-\.'#,/()]+$", RegexOptions.Compiled);

    public string Value { get; }

    private PersonAddressNeighborhood(string value) => Value = value;

    public static PersonAddressNeighborhood Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Neighborhood cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 120)
            throw new ArgumentException("Neighborhood cannot exceed 120 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Neighborhood contains invalid characters.", nameof(value));

        return new PersonAddressNeighborhood(value);
    }

    public override string ToString() => Value;
}
