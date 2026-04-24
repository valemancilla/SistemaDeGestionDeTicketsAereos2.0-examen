// Value Object para el monto de penalización por cancelación: no puede ser negativo ni exceder el límite máximo
namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.valueObject;

public sealed record BookingCancellationPenaltyAmount
{
    // El valor decimal del monto de penalización
    public decimal Value { get; }

    // Constructor privado
    private BookingCancellationPenaltyAmount(decimal value) => Value = value;

    // Valida que la penalización esté dentro del rango permitido y la redondea a 2 decimales
    public static BookingCancellationPenaltyAmount Create(decimal value)
    {
        // La penalización no puede ser negativa (si no hay cargo, se pone 0)
        if (value < 0)
            throw new ArgumentException("Penalty amount cannot be negative.", nameof(value));

        // Coherente con montos de boletería en COP hasta 15 millones
        const decimal maxPenalty = 15_000_000m;
        if (value > maxPenalty)
            throw new ArgumentException($"Penalty amount cannot exceed {maxPenalty:N0} COP.", nameof(value));

        return new BookingCancellationPenaltyAmount(Math.Round(value, 2));
    }

    public override string ToString() => Value.ToString("F2");
}
