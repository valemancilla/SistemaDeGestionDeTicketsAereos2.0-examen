// Registra cada cambio de estado que tuvo un vuelo, quién lo hizo y cuándo ocurrió
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Infrastructure.Entity;

// Entidad que representa la tabla FlightStatusHistory (historial de estados de un vuelo)
public class FlightStatusHistoryEntity
{
    // Clave primaria del registro histórico
    public int IdHistory { get; set; }

    // FK al vuelo cuyo estado cambió
    public int IdFlight { get; set; }

    // FK al nuevo estado que tomó el vuelo
    public int IdStatus { get; set; }

    // Fecha y hora en que se realizó el cambio
    public DateTime ChangeDate { get; set; }

    // FK al usuario que registró el cambio de estado
    public int IdUser { get; set; }

    // Observación opcional sobre el motivo del cambio (ej: "retraso por clima")
    public string? Observation { get; set; }

    // Navegación al vuelo
    public FlightEntity Flight { get; set; } = null!;

    // Navegación al estado registrado
    public SystemStatusEntity Status { get; set; } = null!;

    // Navegación al usuario que hizo el cambio
    public UserEntity User { get; set; } = null!;
}
