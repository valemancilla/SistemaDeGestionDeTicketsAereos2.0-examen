namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.valueObject;

// Value Object para la fecha y hora del cambio de estado del vuelo
public sealed record FlightStatusHistoryChangeDate
{
    // Fecha y hora exacta del cambio — incluye hora porque los cambios pueden ocurrir varias veces al día
    public DateTime Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FlightStatusHistoryChangeDate(DateTime value) => Value = value;

    // Valida que la fecha no esté vacía y que no sea futura
    public static FlightStatusHistoryChangeDate Create(DateTime value)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Change date cannot be empty.", nameof(value));

        if (value > DateTime.Now)
            throw new ArgumentException("Change date cannot be in the future.", nameof(value));

        return new FlightStatusHistoryChangeDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");
}
