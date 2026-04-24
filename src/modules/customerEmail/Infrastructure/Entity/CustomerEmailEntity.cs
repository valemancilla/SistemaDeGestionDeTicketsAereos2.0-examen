// Guarda los correos electrónicos de una persona, que puede tener más de uno registrado
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Infrastructure.Entity;

// Entidad que representa la tabla CustomerEmail (correos de un cliente) en la base de datos
public class CustomerEmailEntity
{
    // Clave primaria del correo
    public int IdEmail { get; set; }

    // FK a la persona dueña de este correo
    public int IdPerson { get; set; }

    // Dirección de correo electrónico
    public string Email { get; set; } = string.Empty;

    // Navegación a la persona
    public PersonEntity Person { get; set; } = null!;
}
