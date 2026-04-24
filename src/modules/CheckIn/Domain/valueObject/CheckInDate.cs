namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.valueObject;

// Value Object para la fecha del check-in, no puede ser vacía ni estar en el futuro
public sealed record CheckInDate
{
    // Fecha y hora en que se realizó el check-in
    public DateTime Value { get; }

    private CheckInDate(DateTime value) => Value = value;

    /// <param name="value">Instante del check-in (se acepta local o UTC).</param>
    /// <param name="referenceUtc">
    /// Instante "ahora" en UTC para la regla «no futuro». Si es null se usa <see cref="DateTime.UtcNow"/>.
    /// Evita el choque Utc vs local cuando la UI pasa <see cref="DateTime.UtcNow"/> y antes se comparaba con <see cref="DateTime.Now"/>.
    /// </param>
    public static CheckInDate Create(DateTime value, DateTime? referenceUtc = null)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Check-in date cannot be empty.", nameof(value));

        var valueUtc = ToUtc(value);
        var refUtc = referenceUtc ?? DateTime.UtcNow;

        // Tolerancia de 2 min: reloj del SO, orden de lectura y tipos datetime sin zona en BD.
        if (valueUtc > refUtc.AddMinutes(2))
            throw new ArgumentException("Check-in date cannot be in the future.", nameof(value));

        return new CheckInDate(valueUtc);
    }

    /// <summary>
    /// Reconstruye desde persistencia sin revalidar «no futuro» (evita falsos positivos al leer).
    /// </summary>
    public static CheckInDate FromStorage(DateTime value)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Check-in date cannot be empty.", nameof(value));
        return new CheckInDate(value);
    }

    private static DateTime ToUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Local).ToUniversalTime(),
        };

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");
}
