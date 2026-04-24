// El modelo de aeronave es producido por un fabricante y es usado por los aviones del sistema
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Infrastructure.Entity;

// Entidad que representa la tabla AircraftModel en la base de datos
// Ejemplos: Boeing 737, Airbus A320, Embraer E190
public class AircraftModelEntity
{
    // Clave primaria del modelo
    public int IdModel { get; set; }

    // FK al fabricante que produce este modelo
    public int IdManufacturer { get; set; }

    // Nombre del modelo (ej: 737-800, A320neo)
    public string Model { get; set; } = string.Empty;

    // Navegación al fabricante
    public ManufacturerEntity Manufacturer { get; set; } = null!;

    // Aviones que son de este modelo
    public ICollection<AircraftEntity> Aircrafts { get; set; } = new List<AircraftEntity>();
}
