// El canal de check-in define por dónde hizo el pasajero su check-in
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Infrastructure.Entity;

// Entidad que representa la tabla CheckInChannel en la base de datos
// Ejemplos: Web, Móvil, Mostrador, Kiosco
public class CheckInChannelEntity
{
    // Clave primaria del canal
    public int IdChannel { get; set; }

    // Nombre del canal (ej: Web, Aplicación móvil, Mostrador del aeropuerto)
    public string ChannelName { get; set; } = string.Empty;

    // Lista de check-ins realizados a través de este canal
    public ICollection<CheckInEntity> CheckIns { get; set; } = new List<CheckInEntity>();
}
