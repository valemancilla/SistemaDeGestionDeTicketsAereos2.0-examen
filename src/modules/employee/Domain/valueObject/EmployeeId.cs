namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.valueObject;

// Value Object para el ID del empleado
public sealed record EmployeeId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private EmployeeId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static EmployeeId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("EmployeeId must be greater than 0.", nameof(value));

        return new EmployeeId(value);
    }

    public override string ToString() => Value.ToString();
}
