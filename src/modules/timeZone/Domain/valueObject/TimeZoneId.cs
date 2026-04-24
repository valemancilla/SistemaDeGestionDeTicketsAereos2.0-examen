namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.valueObject;

// Value Object para el ID de la zona horaria
public sealed record TimeZoneId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private TimeZoneId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static TimeZoneId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("TimeZoneId must be greater than 0.", nameof(value));

        return new TimeZoneId(value);
    }

    public override string ToString() => Value.ToString();
}
