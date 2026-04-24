// Importamos la entidad de Persona porque un género puede estar asociado a muchas personas
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Infrastructure.Entity;

// Esta clase representa la tabla Gender en la base de datos
// Es la entidad que EF Core usa para leer y escribir datos, no confundir con el aggregate del dominio
public class GenderEntity
{
    // Clave primaria de la tabla, se genera automáticamente
    public int IdGender { get; set; }

    // Descripción del género (ej: Masculino, Femenino, Otro)
    public string Description { get; set; } = string.Empty;

    // Navegación inversa: un género puede pertenecer a muchas personas
    public ICollection<PersonEntity> Persons { get; set; } = new List<PersonEntity>();
}
