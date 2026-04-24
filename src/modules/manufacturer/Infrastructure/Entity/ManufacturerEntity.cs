// El fabricante produce los modelos de aeronaves que operan en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Infrastructure.Entity;

// Entidad que representa la tabla Manufacturer en la base de datos
// Ejemplos: Boeing, Airbus, Embraer
public class ManufacturerEntity
{
    // Clave primaria del fabricante
    public int IdManufacturer { get; set; }

    // Nombre del fabricante (ej: Boeing, Airbus)
    public string Name { get; set; } = string.Empty;

    // Modelos de aeronave que produce este fabricante
    public ICollection<AircraftModelEntity> AircraftModels { get; set; } = new List<AircraftModelEntity>();
}
