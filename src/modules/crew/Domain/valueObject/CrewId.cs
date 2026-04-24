namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.valueObject;

// Value Object para el ID del grupo de tripulación
public sealed record CrewId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CrewId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static CrewId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("CrewId must be greater than 0.", nameof(value));

        return new CrewId(value);
    }

    public override string ToString() => Value.ToString();
}
