namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;

// Value Object para la fecha y hora de emisión del tiquete
public sealed record TicketIssueDate
{
    // Incluye hora porque varios tiquetes pueden emitirse el mismo día
    public DateTime Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private TicketIssueDate(DateTime value) => Value = value;

    /// <param name="value">Fecha/hora de emisión del tiquete.</param>
    /// <param name="referenceTime">Instante "ahora" respecto al cual comprobar que no es futuro (evita doble
    /// <see cref="DateTime.Now"/> con microsegundos de diferencia o reloj ajustado entre dos comprobaciones).</param>
    public static TicketIssueDate Create(DateTime value, DateTime? referenceTime = null)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Ticket issue date cannot be empty.", nameof(value));

        var r = referenceTime ?? DateTime.Now;
        if (value > r)
            throw new ArgumentException("Ticket issue date cannot be in the future.", nameof(value));

        return new TicketIssueDate(value);
    }

    /// <summary>
    /// Reconstruye el valor desde persistencia (BD). No revalida «no futuro»: al leer, la hora del servidor,
    /// zona horaria y el tipo <c>datetime</c> de MySQL pueden desalinearse unos segundos frente a
    /// <see cref="DateTime.Now"/> del cliente y disparar un falso positivo.
    /// </summary>
    public static TicketIssueDate FromStorage(DateTime value)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Ticket issue date cannot be empty.", nameof(value));
        return new TicketIssueDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");
}
