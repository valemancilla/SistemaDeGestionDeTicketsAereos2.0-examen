// Una ciudad pertenece a un país y puede tener aeropuertos y direcciones asociadas
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Infrastructure.Entity;

// Entidad que representa la tabla City en la base de datos
public class CityEntity
{
    // Identificador único de la ciudad
    public int IdCity { get; set; }

    // Nombre de la ciudad (ej: Bogotá, Medellín)
    public string Name { get; set; } = string.Empty;

    // FK hacia el país al que pertenece esta ciudad
    public int IdCountry { get; set; }

    // Navegación hacia el país (relación muchos a uno)
    public CountryEntity Country { get; set; } = null!;

    // Aeropuertos que están en esta ciudad
    public ICollection<AirportEntity> Airports { get; set; } = new List<AirportEntity>();

    // Direcciones de personas que viven en esta ciudad
    public ICollection<PersonAddressEntity> PersonAddresses { get; set; } = new List<PersonAddressEntity>();
}
