// Registra cada cambio de estado que tuvo un ticket, con quién lo hizo y en qué momento
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Entity;

// Entidad que representa la tabla TicketStatusHistory (historial de estados de un ticket)
public class TicketStatusHistoryEntity
{
    // Clave primaria del registro histórico
    public int IdHistory { get; set; }

    // FK al ticket cuyo estado cambió
    public int IdTicket { get; set; }

    // FK al estado que tomó el ticket en este momento
    public int IdStatus { get; set; }

    // Fecha y hora del cambio de estado
    public DateTime ChangeDate { get; set; }

    // FK al usuario que realizó el cambio
    public int IdUser { get; set; }

    // Observación opcional que explica el motivo del cambio
    public string? Observation { get; set; }

    // Navegación al ticket
    public TicketEntity Ticket { get; set; } = null!;

    // Navegación al estado registrado
    public SystemStatusEntity Status { get; set; } = null!;

    // Navegación al usuario que hizo el cambio
    public UserEntity User { get; set; } = null!;
}
