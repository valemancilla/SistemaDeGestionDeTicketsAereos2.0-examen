// El usuario tiene acceso al sistema con un rol específico y puede estar vinculado a una persona
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

// Entidad que representa la tabla User (usuario del sistema) en la base de datos
public class UserEntity
{
    // Clave primaria del usuario
    public int IdUser { get; set; }

    // Nombre de usuario para iniciar sesión (único en el sistema)
    public string Username { get; set; } = string.Empty;

    // Contraseña del usuario (debe almacenarse cifrada)
    public string Password { get; set; } = string.Empty;

    // FK al rol del usuario (Administrador, Cliente...)
    public int IdUserRole { get; set; }

    // FK opcional a la persona con los datos personales del usuario
    public int? IdPerson { get; set; }

    // Indica si el usuario está activo en el sistema
    public bool Active { get; set; }

    // Navegación al rol del usuario
    public RoleEntity Role { get; set; } = null!;

    // Navegación a la persona vinculada (puede ser null si no se vinculó)
    public PersonEntity? Person { get; set; }

    // Reservas donde este usuario aparece como cliente
    public ICollection<BookingCustomerEntity> BookingCustomers { get; set; } = new List<BookingCustomerEntity>();

    // Cambios de estado de vuelos que hizo este usuario
    public ICollection<FlightStatusHistoryEntity> FlightStatusHistories { get; set; } = new List<FlightStatusHistoryEntity>();

    // Cambios de estado de reservas que hizo este usuario
    public ICollection<BookingStatusHistoryEntity> BookingStatusHistories { get; set; } = new List<BookingStatusHistoryEntity>();

    // Cambios de estado de tickets que hizo este usuario
    public ICollection<TicketStatusHistoryEntity> TicketStatusHistories { get; set; } = new List<TicketStatusHistoryEntity>();

    // Cancelaciones de reservas que procesó este usuario
    public ICollection<BookingCancellationEntity> BookingCancellations { get; set; } = new List<BookingCancellationEntity>();

    // Check-ins que procesó este usuario
    public ICollection<CheckInEntity> CheckIns { get; set; } = new List<CheckInEntity>();
}
