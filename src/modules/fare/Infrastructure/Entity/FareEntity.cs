// La tarifa define el precio base que cobra una aerolínea y tiene un período de vigencia
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;

// Entidad que representa la tabla Fare (tarifa) en la base de datos
public class FareEntity
{
    // Clave primaria de la tarifa
    public int IdFare { get; set; }

    // Nombre descriptivo de la tarifa (ej: "Económica", "Business", "Promocional")
    public string FareName { get; set; } = string.Empty;

    // Precio base de la tarifa en la moneda del sistema
    public decimal BasePrice { get; set; }

    // FK a la aerolínea que ofrece esta tarifa
    public int IdAirline { get; set; }

    // Fecha desde la que esta tarifa es válida
    public DateOnly ValidFrom { get; set; }

    // Fecha hasta la que esta tarifa es válida
    public DateOnly ValidTo { get; set; }

    // Indica si la tarifa está activa actualmente
    public bool Active { get; set; }

    // Fecha de vencimiento opcional (puede dejarse en null si no expira)
    public DateOnly? ExpirationDate { get; set; }

    // Navegación a la aerolínea que ofrece la tarifa
    public AerolineEntity Airline { get; set; } = null!;

    // Tickets que se han vendido usando esta tarifa
    public ICollection<TicketEntity> Tickets { get; set; } = new List<TicketEntity>();

    // Precios por clase de asiento (económica/ejecutiva/primera, etc.)
    public ICollection<FareSeatClassPriceEntity> SeatClassPrices { get; set; } = new List<FareSeatClassPriceEntity>();
}
