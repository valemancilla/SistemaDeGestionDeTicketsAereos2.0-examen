using System.Text.RegularExpressions;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.valueObject;

// Value Object para el nombre del método de pago, solo letras con acentos, espacios y guiones
public sealed record PaymentMethodName
{
    // Ej: "Tarjeta de Crédito", "Efectivo", "PSE", "Transferencia Bancaria"
    private static readonly Regex ValidPattern = new(@"^[a-zA-ZÀ-ÿ\s\-]+$", RegexOptions.Compiled);

    // El valor del nombre del método de pago
    public string Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private PaymentMethodName(string value) => Value = value;

    // Valida que el nombre no esté vacío, no exceda 50 caracteres y tenga caracteres válidos
    public static PaymentMethodName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Payment method name cannot be empty.", nameof(value));

        value = value.Trim();

        if (value.Length > 50)
            throw new ArgumentException("Payment method name cannot exceed 50 characters.", nameof(value));

        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException("Payment method name contains invalid characters.", nameof(value));

        return new PaymentMethodName(value);
    }

    public override string ToString() => Value;
}
