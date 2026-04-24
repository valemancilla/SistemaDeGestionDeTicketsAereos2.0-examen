// El equipaje está asociado a un ticket específico y tiene un tipo que define sus características
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Entity;

// Entidad que representa la tabla Baggage (equipaje) en la base de datos
public class BaggageEntity
{
    // Clave primaria del equipaje
    public int IdBaggage { get; set; }

    // FK al ticket al que pertenece este equipaje
    public int IdTicket { get; set; }

    // FK al tipo de equipaje (cabina, bodega, etc.)
    public int IdBaggageType { get; set; }

    // Peso del equipaje en kilogramos
    public decimal Weight { get; set; }

    // Navegación al ticket correspondiente
    public TicketEntity Ticket { get; set; } = null!;

    // Navegación al tipo de equipaje
    public BaggageTypeEntity BaggageType { get; set; } = null!;
}
