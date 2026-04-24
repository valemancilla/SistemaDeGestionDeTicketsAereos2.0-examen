namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.valueObject;

// Value Object para el ID del tipo de documento
public sealed record DocumentTypeId
{
    // El valor entero del ID
    public int Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private DocumentTypeId(int value) => Value = value;

    // Valida que el ID no sea negativo
    public static DocumentTypeId Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("DocumentTypeId must be greater than 0.", nameof(value));

        return new DocumentTypeId(value);
    }

    public override string ToString() => Value.ToString();
}
