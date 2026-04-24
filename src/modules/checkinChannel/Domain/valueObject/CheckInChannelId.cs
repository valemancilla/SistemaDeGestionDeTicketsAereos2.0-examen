namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.valueObject;

// Value Object para el ID del canal de check-in
public sealed record CheckInChannelId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CheckInChannelId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static CheckInChannelId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("CheckInChannelId must be greater than 0.", nameof(value));

        return new CheckInChannelId(value);
    }

    public override string ToString() => Value.ToString();
}
