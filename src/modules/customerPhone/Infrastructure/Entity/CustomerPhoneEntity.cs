// Guarda los teléfonos de una persona, que puede tener varios números registrados
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Infrastructure.Entity;

// Entidad que representa la tabla CustomerPhone (teléfonos de un cliente) en la base de datos
public class CustomerPhoneEntity
{
    // Clave primaria del teléfono
    public int IdPhone { get; set; }

    // FK a la persona dueña de este teléfono
    public int IdPerson { get; set; }

    // Número de teléfono (puede incluir código de país)
    public string Phone { get; set; } = string.Empty;

    // Navegación a la persona
    public PersonEntity Person { get; set; } = null!;
}
