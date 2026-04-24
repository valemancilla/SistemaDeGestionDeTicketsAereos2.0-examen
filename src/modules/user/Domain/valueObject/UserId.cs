namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.valueObject;

// Value Object para el ID del usuario del sistema
public sealed record UserId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private UserId(int value) => Value = value;

    // Valida que el ID no sea negativo (0 es válido cuando la BD aún no ha asignado un ID real)
    public static UserId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("UserId must be greater than 0.", nameof(value));

        return new UserId(value);
    }

    public override string ToString() => Value.ToString();
}
