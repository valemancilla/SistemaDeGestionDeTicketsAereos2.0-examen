namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;

// Tipo de vivienda: casa o apartamento
public sealed record PersonAddressDwellingType
{
    public string Value { get; }

    private PersonAddressDwellingType(string value) => Value = value;

    public static PersonAddressDwellingType Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Dwelling type cannot be empty.", nameof(value));

        var v = value.Trim();
        if (string.Equals(v, "Casa", StringComparison.OrdinalIgnoreCase))
            return new PersonAddressDwellingType("Casa");

        if (string.Equals(v, "Apartamento", StringComparison.OrdinalIgnoreCase))
            return new PersonAddressDwellingType("Apartamento");

        throw new ArgumentException("Dwelling type must be Casa or Apartamento.", nameof(value));
    }

    public override string ToString() => Value;
}
