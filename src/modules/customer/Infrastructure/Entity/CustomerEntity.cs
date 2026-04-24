// El cliente está ligado a una persona con sus datos personales y guarda la fecha en que se registró
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Infrastructure.Entity;

// Entidad que representa la tabla Customer (cliente del sistema) en la base de datos
public class CustomerEntity
{
    // Clave primaria del cliente
    public int IdCustomer { get; set; }

    // FK a la persona que es el cliente (datos personales)
    public int IdPerson { get; set; }

    // Indica si el cliente está activo en el sistema
    public bool Active { get; set; }

    // Fecha en que el cliente se registró por primera vez
    public DateOnly RegistrationDate { get; set; }

    // Navegación a la persona con los datos del cliente
    public PersonEntity Person { get; set; } = null!;
}
