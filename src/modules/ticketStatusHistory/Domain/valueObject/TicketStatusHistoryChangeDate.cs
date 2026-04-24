namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.valueObject;

// Value Object para la fecha y hora del cambio de estado del tiquete
public sealed record TicketStatusHistoryChangeDate
{
    // Incluye hora porque varios cambios pueden ocurrir el mismo día
    public DateTime Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private TicketStatusHistoryChangeDate(DateTime value) => Value = value;

    // Valida que la fecha no esté vacía y que no sea futura
    public static TicketStatusHistoryChangeDate Create(DateTime value)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Change date cannot be empty.", nameof(value));

        if (value > DateTime.Now)
            throw new ArgumentException("Change date cannot be in the future.", nameof(value));

        return new TicketStatusHistoryChangeDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");
}
