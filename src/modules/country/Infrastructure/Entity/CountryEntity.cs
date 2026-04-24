// Un país tiene ciudades, aerolíneas y personas asociadas
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Entity;

// Entidad que representa la tabla Country en la base de datos
public class CountryEntity
{
    // Identificador único del país (clave primaria)
    public int IdCountry { get; set; }

    // Nombre completo del país (ej: Colombia, México)
    public string Name { get; set; } = string.Empty;

    // Código ISO de 2 letras del país (ej: CO, MX, US) — debe ser único
    public string ISOCode { get; set; } = string.Empty;

    // Lista de ciudades que pertenecen a este país
    public ICollection<CityEntity> Cities { get; set; } = new List<CityEntity>();

    // Lista de aerolíneas registradas en este país
    public ICollection<AerolineEntity> Airlines { get; set; } = new List<AerolineEntity>();

    // Lista de personas que son de este país
    public ICollection<PersonEntity> Persons { get; set; } = new List<PersonEntity>();
}
