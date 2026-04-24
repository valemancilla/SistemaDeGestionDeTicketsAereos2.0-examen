namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.valueObject;

// Value Object para la duración estimada del vuelo en la ruta (en formato HH:mm)
public sealed record RouteEstDuration
{
    // TimeOnly porque la duración se expresa como horas y minutos, no como fecha
    public TimeOnly Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private RouteEstDuration(TimeOnly value) => Value = value;

    // Valida que la duración no sea cero — un vuelo siempre tarda algo
    public static RouteEstDuration Create(TimeOnly value)
    {
        if (value == TimeOnly.MinValue)
            throw new ArgumentException("Estimated duration cannot be zero.", nameof(value));

        return new RouteEstDuration(value);
    }

    public override string ToString() => Value.ToString("HH:mm");
}
