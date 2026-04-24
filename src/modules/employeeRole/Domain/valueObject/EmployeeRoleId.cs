namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.valueObject;

// Value Object para el ID del rol de empleado
public sealed record EmployeeRoleId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private EmployeeRoleId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static EmployeeRoleId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("EmployeeRoleId must be greater than 0.", nameof(value));

        return new EmployeeRoleId(value);
    }

    public override string ToString() => Value.ToString();
}
