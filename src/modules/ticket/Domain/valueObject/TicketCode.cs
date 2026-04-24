using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;

// Value Object para el código único del tiquete (ej: "TK009", "TK20241234AB")
public sealed record TicketCode
{
    // Entre 5 y 20 caracteres alfanuméricos en mayúsculas — formato interno del sistema
    private static readonly Regex ValidPattern = new(@"^[A-Z0-9]{5,20}$", RegexOptions.Compiled);

    // El valor del código, normalizado a mayúsculas
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private TicketCode(string value) => Value = value;

    // Valida el formato del código y lo normaliza a mayúsculas
    public static TicketCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Ticket code cannot be empty.", nameof(value));

        value = value.Trim().ToUpper();

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Ticket code must be 5 to 20 uppercase alphanumeric characters.", nameof(value));

        return new TicketCode(value);
    }

    public override string ToString() => Value;
}
