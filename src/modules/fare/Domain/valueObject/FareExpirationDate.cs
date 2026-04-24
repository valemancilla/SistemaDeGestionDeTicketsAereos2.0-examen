namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;

// Value Object para la fecha de expiración anticipada de la tarifa (opcional)
public sealed record FareExpirationDate
{
    // Puede ser null si la tarifa no tiene expiración anticipada definida
    public DateOnly? Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private FareExpirationDate(DateOnly? value) => Value = value;

    // No tiene validaciones propias — la coherencia con ValidTo se verifica en el agregado
    public static FareExpirationDate Create(DateOnly? value)
    {
        return new FareExpirationDate(value);
    }

    public override string ToString() => Value?.ToString("yyyy-MM-dd") ?? string.Empty;
}
