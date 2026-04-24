// El canal de check-in indica cómo el pasajero realizó el proceso (web, mostrador, app móvil...)
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.aggregate;

// Agregado CheckInChannel: encapsula las reglas de negocio de un canal de check-in
public class CheckInChannel
{
    // ID del canal (Value Object)
    public CheckInChannelId Id { get; private set; }

    // Nombre del canal (ej: "Web", "Mostrador", "App Móvil")
    public CheckInChannelName Name { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private CheckInChannel(CheckInChannelId id, CheckInChannelName name)
    {
        Id = id;
        Name = name;
    }

    // Método de fábrica para crear o reconstruir un canal de check-in
    public static CheckInChannel Create(int id, string name)
    {
        // Regla: el nombre del canal es validado por su Value Object (no vacío)
        return new CheckInChannel(
            CheckInChannelId.Create(id),
            CheckInChannelName.Create(name)
        );
    }

    // Método de fábrica para crear un canal nuevo (ID = 0, la BD lo asigna después)
    public static CheckInChannel CreateNew(string name) => Create(0, name);
}
