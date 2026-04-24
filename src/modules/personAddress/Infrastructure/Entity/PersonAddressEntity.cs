// Guarda las direcciones físicas de una persona, que puede tener más de una registrada
using SistemaDeGestionDeTicketsAereos.src.modules.city.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Infrastructure.Entity;

// Entidad que representa la tabla PersonAddress (dirección de una persona) en la base de datos
public class PersonAddressEntity
{
    // Clave primaria de la dirección
    public int IdAddress { get; set; }

    // FK a la persona dueña de esta dirección
    public int IdPerson { get; set; }

    // Dirección completa de vía y placa (ej. Calle 31 # 21-56)
    public string Street { get; set; } = string.Empty;

    // Interior, apartamento, unidad o número de casa
    public string Number { get; set; } = string.Empty;

    // Barrio, sector o conjunto
    public string Neighborhood { get; set; } = string.Empty;

    // Casa o Apartamento
    public string DwellingType { get; set; } = string.Empty;

    // FK a la ciudad donde se ubica esta dirección
    public int IdCity { get; set; }

    // Código postal opcional (no todos los países lo usan)
    public string? ZipCode { get; set; }

    // Indica si esta dirección está vigente o fue reemplazada por otra
    public bool Active { get; set; }

    // Navegación a la persona dueña de la dirección
    public PersonEntity Person { get; set; } = null!;

    // Navegación a la ciudad de la dirección
    public CityEntity City { get; set; } = null!;
}
