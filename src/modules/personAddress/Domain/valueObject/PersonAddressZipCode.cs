using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;

// Value Object para el código postal de la dirección (opcional — no todos los países lo usan igual)
public sealed record PersonAddressZipCode
{
    // Entre 3 y 10 caracteres alfanuméricos con guiones — cubre formatos de Colombia, US, UK, etc.
    private static readonly Regex ValidPattern = new(@"^[a-zA-Z0-9\-]{3,10}$", RegexOptions.Compiled);

    // Puede ser null si la persona no tiene código postal en su dirección
    public string? Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PersonAddressZipCode(string? value) => Value = value;

    // Si el valor es null o vacío lo acepta como null — si tiene contenido, lo valida
    public static PersonAddressZipCode Create(string? value)
    {
        if (value == null)
            return new PersonAddressZipCode((string?)null);

        value = value.Trim().ToUpper();

        // Si queda vacío después del trim, lo tratamos como null
        if (value.Length == 0)
            return new PersonAddressZipCode((string?)null);

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Zip code must be 3 to 10 alphanumeric characters (hyphens allowed).", nameof(value));

        return new PersonAddressZipCode(value);
    }

    public override string ToString() => Value ?? string.Empty;
}
