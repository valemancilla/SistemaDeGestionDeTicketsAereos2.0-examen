// El estado del sistema es un catálogo compartido que clasifica el estado de vuelos, reservas y tiquetes
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.aggregate;

// Agregado SystemStatus: encapsula las reglas de negocio de un estado del sistema
public class SystemStatus
{
    // ID del estado (Value Object)
    public SystemStatusId Id { get; private set; }

    // Nombre del estado (ej: "Activo", "Cancelado", "Completado", "Pendiente")
    public SystemStatusName Name { get; private set; }

    // Tipo de entidad a la que aplica este estado (ej: "Flight", "Booking", "Ticket")
    public SystemStatusEntityType EntityType { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private SystemStatus(SystemStatusId id, SystemStatusName name, SystemStatusEntityType entityType)
    {
        Id = id;
        Name = name;
        EntityType = entityType;
    }

    // Método de fábrica para crear o reconstruir un estado del sistema desde la base de datos
    public static SystemStatus Create(int id, string name, string entityType)
    {
        // Regla: nombre y tipo de entidad son validados por sus Value Objects (no vacíos)
        return new SystemStatus(
            SystemStatusId.Create(id),
            SystemStatusName.Create(name),
            SystemStatusEntityType.Create(entityType)
        );
    }

    // Método de fábrica para crear un estado nuevo (ID = 0, la BD lo asigna después)
    public static SystemStatus CreateNew(string name, string entityType) => Create(0, name, entityType);
}
