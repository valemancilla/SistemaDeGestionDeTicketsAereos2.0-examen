namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;

// Value Object para la fecha de nacimiento de la persona
public sealed record PersonBirthDate
{
    // La fecha de nacimiento como DateOnly (sin hora)
    public DateOnly Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PersonBirthDate(DateOnly value) => Value = value;

    // Valida que la fecha sea real, pasada y no más antigua de 120 años
    public static PersonBirthDate Create(DateOnly value)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (value == DateOnly.MinValue)
            throw new ArgumentException("Birth date cannot be empty.", nameof(value));

        // Una persona no puede haber nacido hoy o después
        if (value >= today)
            throw new ArgumentException("Birth date must be in the past.", nameof(value));

        // Límite de 120 años para evitar datos basura
        if (today.Year - value.Year > 120)
            throw new ArgumentException("Birth date cannot be more than 120 years ago.", nameof(value));

        return new PersonBirthDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");
}
