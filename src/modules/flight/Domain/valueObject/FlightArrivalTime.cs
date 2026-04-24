namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

// Value Object para la hora de llegada del vuelo al destino
public sealed record FlightArrivalTime
{
    // La hora de aterrizaje — la coherencia con DepartureTime se verifica en el agregado
    public TimeOnly Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FlightArrivalTime(TimeOnly value) => Value = value;

    // No requiere validaciones propias ya que TimeOnly siempre es una hora válida
    public static FlightArrivalTime Create(TimeOnly value)
    {
        return new FlightArrivalTime(value);
    }

    public override string ToString() => Value.ToString("HH:mm");
}
