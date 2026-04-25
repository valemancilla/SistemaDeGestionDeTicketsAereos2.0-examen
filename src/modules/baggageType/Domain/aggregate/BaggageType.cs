// El tipo de equipaje categoriza el equipaje del pasajero (ej: de mano, bodega, deportivo)
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.aggregate;

// Agregado BaggageType: encapsula las reglas de negocio de un tipo de equipaje
public class BaggageType
{
    // ID del tipo de equipaje (Value Object)
    public BaggageTypeId Id { get; private set; }

    // Nombre del tipo de equipaje (Value Object con validación)
    public BaggageTypeName Name { get; private set; }

    public decimal WeightKg { get; private set; }

    public decimal BasePriceCop { get; private set; }

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private BaggageType(
        BaggageTypeId id,
        BaggageTypeName name,
        decimal weightKg,
        decimal basePriceCop,
        string? description,
        bool isActive)
    {
        Id = id;
        Name = name;
        WeightKg = weightKg;
        BasePriceCop = basePriceCop;
        Description = description;
        IsActive = isActive;
    }

    // Método de fábrica para crear o reconstruir un tipo de equipaje
    public static BaggageType Create(int id, string name)
    {
        return Create(id, name, weightKg: 0m, basePriceCop: 0m, description: null, isActive: true);
    }

    public static BaggageType Create(
        int id,
        string name,
        decimal weightKg,
        decimal basePriceCop,
        string? description,
        bool isActive)
    {
        if (weightKg < 0) throw new ArgumentOutOfRangeException(nameof(weightKg));
        if (basePriceCop < 0) throw new ArgumentOutOfRangeException(nameof(basePriceCop));
        if (description is not null && description.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters.", nameof(description));

        return new BaggageType(
            BaggageTypeId.Create(id),
            BaggageTypeName.Create(name),
            decimal.Round(weightKg, 2),
            decimal.Round(basePriceCop, 2),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            isActive);
    }

    // Método de fábrica para crear un tipo nuevo (ID = 0, la BD lo asigna después)
    public static BaggageType CreateNew(string name) => Create(0, name);

    public static BaggageType CreateNew(
        string name,
        decimal weightKg,
        decimal basePriceCop,
        string? description,
        bool isActive) => Create(0, name, weightKg, basePriceCop, description, isActive);
}
