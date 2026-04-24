// La aerolínea pertenece a un país y opera aviones, empleados y tarifas
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Entity;

// Entidad que representa la tabla Airline en la base de datos
// Nota: la tabla en BD se llama "Airline" pero el módulo se llama "aeroline" para evitar conflictos de nombre
public sealed class AerolineEntity
{
    // Clave primaria de la aerolínea
    public int IdAirline { get; set; }

    // Nombre completo de la aerolínea (ej: Avianca, LATAM Airlines)
    public string Name { get; set; } = string.Empty;

    // Código IATA de 2 letras que identifica a la aerolínea mundialmente (ej: AV, LA)
    public string IATACode { get; set; } = string.Empty;

    // FK al país donde está registrada la aerolínea
    public int IdCountry { get; set; }

    // Indica si la aerolínea está operando actualmente
    public bool Active { get; set; }

    // Navegación al país
    public CountryEntity Country { get; set; } = null!;

    // Aviones que pertenecen a esta aerolínea
    public ICollection<AircraftEntity> Aircrafts { get; set; } = new List<AircraftEntity>();

    // Empleados que trabajan para esta aerolínea
    public ICollection<EmployeeEntity> Employees { get; set; } = new List<EmployeeEntity>();

    // Tarifas que ofrece esta aerolínea
    public ICollection<FareEntity> Fares { get; set; } = new List<FareEntity>();
}
