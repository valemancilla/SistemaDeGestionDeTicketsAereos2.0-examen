namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.valueObject;

// Value Object para el ID del fabricante de aeronaves
public sealed record ManufacturerId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private ManufacturerId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static ManufacturerId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("ManufacturerId must be greater than 0.", nameof(value));

        return new ManufacturerId(value);
    }

    public override string ToString() => Value.ToString();
}
