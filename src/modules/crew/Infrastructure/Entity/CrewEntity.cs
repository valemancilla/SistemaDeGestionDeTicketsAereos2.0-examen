// La tripulación agrupa a los empleados que operan un vuelo, como el piloto, copiloto y auxiliares
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Infrastructure.Entity;

// Entidad que representa la tabla Crew (tripulación) en la base de datos
public class CrewEntity
{
    // Clave primaria de la tripulación
    public int IdCrew { get; set; }

    // Nombre del grupo de tripulación (ej: "Equipo Alpha", "Tripulación 101")
    public string GroupName { get; set; } = string.Empty;

    // Vuelos en los que opera esta tripulación
    public ICollection<FlightEntity> Flights { get; set; } = new List<FlightEntity>();

    // Miembros que forman parte de esta tripulación
    public ICollection<CrewMemberEntity> CrewMembers { get; set; } = new List<CrewMemberEntity>();
}
