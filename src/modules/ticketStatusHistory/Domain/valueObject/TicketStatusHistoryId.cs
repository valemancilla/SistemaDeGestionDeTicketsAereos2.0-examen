namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.valueObject;

// Value Object para el ID del registro de historial de estado de tiquete
public sealed record TicketStatusHistoryId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private TicketStatusHistoryId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static TicketStatusHistoryId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("TicketStatusHistoryId must be greater than 0.", nameof(value));

        return new TicketStatusHistoryId(value);
    }

    public override string ToString() => Value.ToString();
}
