// La persona concentra los datos personales de cualquier individuo en el sistema (cliente, empleado o usuario)
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

// Entidad que representa la tabla Person (persona) en la base de datos
public class PersonEntity
{
    // Clave primaria de la persona
    public int IdPerson { get; set; }

    // FK al tipo de documento de identidad (cédula, pasaporte, etc.)
    public int IdDocumentType { get; set; }

    // Número del documento de identidad
    public string DocumentNumber { get; set; } = string.Empty;

    // Primer nombre de la persona
    public string FirstName { get; set; } = string.Empty;

    // Apellido de la persona
    public string LastName { get; set; } = string.Empty;

    // Fecha de nacimiento de la persona
    public DateOnly BirthDate { get; set; }

    // FK al género de la persona
    public int IdGender { get; set; }

    // FK al país de origen o residencia de la persona
    public int IdCountry { get; set; }

    // FK opcional a la dirección actual de la persona (puede ser null)
    public int? IdAddress { get; set; }

    // Navegación al tipo de documento
    public DocumentTypeEntity DocumentType { get; set; } = null!;

    // Navegación al género
    public GenderEntity Gender { get; set; } = null!;

    // Navegación al país
    public CountryEntity Country { get; set; } = null!;

    // La dirección "actual" seleccionada entre las que tiene la persona (es opcional)
    // Una persona puede tener varias direcciones, pero solo una se marca como la actual con este FK
    public PersonAddressEntity? CurrentAddress { get; set; }

    // Todas las direcciones registradas de la persona
    public ICollection<PersonAddressEntity> Addresses { get; set; } = new List<PersonAddressEntity>();

    // Si la persona es cliente, aquí está la navegación
    public CustomerEntity? Customer { get; set; }

    // Teléfonos registrados de esta persona
    public ICollection<CustomerPhoneEntity> Phones { get; set; } = new List<CustomerPhoneEntity>();

    // Correos electrónicos registrados de esta persona
    public ICollection<CustomerEmailEntity> Emails { get; set; } = new List<CustomerEmailEntity>();

    // Reservas en las que aparece esta persona como pasajero
    public ICollection<BookingCustomerEntity> BookingCustomers { get; set; } = new List<BookingCustomerEntity>();

    // Usuarios del sistema vinculados a esta persona
    public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();

    // Empleados vinculados a esta persona (puede ser empleado y cliente al mismo tiempo)
    public ICollection<EmployeeEntity> Employees { get; set; } = new List<EmployeeEntity>();
}
