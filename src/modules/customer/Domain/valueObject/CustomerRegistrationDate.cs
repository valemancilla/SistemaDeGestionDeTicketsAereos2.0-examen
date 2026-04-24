namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.valueObject;

// Value Object para la fecha de registro del cliente en el sistema
public sealed record CustomerRegistrationDate
{
    // La fecha de registro como DateOnly (sin hora)
    public DateOnly Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private CustomerRegistrationDate(DateOnly value) => Value = value;

    // Valida que la fecha sea real (la regla de negocio "posterior a hoy" aplica al crear/actualizar en casos de uso)
    public static CustomerRegistrationDate Create(DateOnly value)
    {
        if (value == DateOnly.MinValue)
            throw new ArgumentException("La fecha de registro no es válida.", nameof(value));

        return new CustomerRegistrationDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");
}
