using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Domain.valueObject;

// Value Object para el nombre del rol de empleado, solo letras con acentos, espacios y guiones
public sealed record EmployeeRoleName
{
    // Ej: "Piloto", "Auxiliar de Vuelo", "Agente de Mostrador"
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-]+$", RegexOptions.Compiled);

    // El valor del nombre del rol
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private EmployeeRoleName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 50 caracteres y tenga caracteres válidos
    public static EmployeeRoleName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Employee role name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 50)
            throw new ArgumentException("Employee role name cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Employee role name contains invalid characters.", nameof(value));

        return new EmployeeRoleName(value);
    }

    public override string ToString() => Value;
}
