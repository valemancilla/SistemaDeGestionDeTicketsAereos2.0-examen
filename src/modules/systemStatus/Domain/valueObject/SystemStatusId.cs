namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.valueObject;

// Value Object para el ID del estado del sistema
public sealed record SystemStatusId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private SystemStatusId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static SystemStatusId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("SystemStatusId must be greater than 0.", nameof(value));

        return new SystemStatusId(value);
    }

    public override string ToString() => Value.ToString();
}
