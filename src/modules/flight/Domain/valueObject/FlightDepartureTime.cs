namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

// Value Object para la hora de salida del vuelo
public sealed record FlightDepartureTime
{
    // La hora de despegue — la coherencia con ArrivalTime se verifica en el agregado
    public TimeOnly Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FlightDepartureTime(TimeOnly value) => Value = value;

    // No requiere validaciones propias ya que TimeOnly siempre es una hora válida
    public static FlightDepartureTime Create(TimeOnly value)
    {
        return new FlightDepartureTime(value);
    }

    public override string ToString() => Value.ToString("HH:mm");
}
