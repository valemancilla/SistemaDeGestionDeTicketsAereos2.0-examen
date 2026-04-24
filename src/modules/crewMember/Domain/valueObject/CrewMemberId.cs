namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.valueObject;

// Value Object para el ID del miembro de tripulación
public sealed record CrewMemberId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CrewMemberId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static CrewMemberId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("CrewMemberId must be greater than 0.", nameof(value));

        return new CrewMemberId(value);
    }

    public override string ToString() => Value.ToString();
}
