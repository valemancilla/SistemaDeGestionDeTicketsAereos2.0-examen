namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;

// Value Object para el ID del tiquete
public sealed record TicketId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private TicketId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static TicketId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("TicketId must be greater than 0.", nameof(value));

        return new TicketId(value);
    }

    public override string ToString() => Value.ToString();
}
