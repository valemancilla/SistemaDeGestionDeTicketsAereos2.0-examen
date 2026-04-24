namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.valueObject;

// Value Object para el ID del rol de usuario
public sealed record RoleId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private RoleId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static RoleId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("RoleId must be greater than 0.", nameof(value));

        return new RoleId(value);
    }

    public override string ToString() => Value.ToString();
}
