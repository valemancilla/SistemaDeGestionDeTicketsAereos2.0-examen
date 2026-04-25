// El tipo de equipaje clasifica el equipaje que lleva un pasajero
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Infrastructure.Entity;

// Entidad que representa la tabla BaggageType en la base de datos
// Ejemplos: Equipaje de mano, Equipaje de bodega, Equipo especial
public class BaggageTypeEntity
{
    // Clave primaria del tipo de equipaje
    public int IdBaggageType { get; set; }

    // Nombre del tipo (ej: Equipaje de mano, Equipaje de bodega)
    public string TypeName { get; set; } = string.Empty;

    // Peso de referencia del tipo (kg). Para bolso/artículo personal puede ser 0.
    public decimal WeightKg { get; set; }

    // Precio base del tipo (COP). Para incluido puede ser 0.
    public decimal BasePriceCop { get; set; }

    // Descripción opcional (para mostrar en administración).
    public string? Description { get; set; }

    // Activo/Inactivo: el admin controla qué tipos se ofrecen.
    public bool IsActive { get; set; } = true;

    // Lista de equipajes que son de este tipo
    public ICollection<BaggageEntity> Baggages { get; set; } = new List<BaggageEntity>();
}
